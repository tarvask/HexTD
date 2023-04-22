using System;
using HexSystem;
using Match.Field;
using Match.Field.Hand;
using Match.Field.Hexagons;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Match.Windows.Tower;
using PathSystem;
using Services;
using Tools;
using Tools.Interfaces;

namespace Match.Commands
{
    public class FieldClicksDistributor : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public FieldClicksHandler ClicksHandler { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public FieldConstructionProcessController ConstructionProcessController { get; }
            public PlayerHandController PlayerHandController { get; }
            public MatchCommands MatchCommands { get; }
            
            public TowerManipulationWindowController TowerManipulationWindowController { get; }
            public TowerInfoWindowController TowerInfoWindowController { get; }
            
            public HexagonalFieldModel HexagonalFieldModel { get; }
            public PathContainer PathContainer { get; }

            public Context(
                FieldModel fieldModel, FieldClicksHandler clicksHandler, 
                ConfigsRetriever configsRetriever,
                FieldConstructionProcessController constructionProcessController,
                PlayerHandController playerHandController,
                MatchCommands matchCommands,
                TowerManipulationWindowController towerManipulationWindowController,
                TowerInfoWindowController towerInfoWindowController,
                HexagonalFieldModel hexagonalFieldModel,
                PathContainer pathContainer)
            {
                FieldModel = fieldModel;
                ClicksHandler = clicksHandler;
                ConfigsRetriever = configsRetriever;
                ConstructionProcessController = constructionProcessController;
                PlayerHandController = playerHandController;
                MatchCommands = matchCommands;
                
                TowerManipulationWindowController = towerManipulationWindowController;
                TowerInfoWindowController = towerInfoWindowController;
                HexagonalFieldModel = hexagonalFieldModel;
                PathContainer = pathContainer;
            }
        }

        private readonly Context _context;

        public FieldClicksDistributor(Context context)
        {
            _context = context;
            
            //_context.MatchCommands.Incoming.ApplyBuildTower.Subscribe(ProcessBuild);
            _context.MatchCommands.Incoming.ApplyUpgradeTower.Subscribe(ProcessUpgrade);
            _context.MatchCommands.Incoming.ApplySellTower.Subscribe(ProcessSell);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            if (_context.ClicksHandler.ClickedCellsCount > 0)
            {
                Hex2d clickedCell = _context.ClicksHandler.GetClickedCell();

                DistributeClick(clickedCell);
            }
        }

        private void DistributeClick(Hex2d clickedCell)
        {
            switch (_context.FieldModel.GetFieldHexType(clickedCell))
            {
                case FieldHexType.Free:
                    //ProcessPreBuild(clickedCell);
                    break;
                
                case FieldHexType.Tower:
                    ProcessPreManipulation(clickedCell);
                    break;
            }
        }

        private void ProcessPreBuild(Hex2d clickedHex)
        {
            if(!_context.PlayerHandController.IsTowerChoice)
                return;
            
            if(_context.HexagonalFieldModel.CurrentFieldHexTypes.GetFieldHexType(clickedHex) != FieldHexType.Free)
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(
                _context.PlayerHandController.ChosenTowerType);

            var isBlocker = _context.HexagonalFieldModel.GetHexIsBlocker(clickedHex);
            if (isBlocker)
            {
                return;
            }
            
            var isRoad = _context.PathContainer.GetHexIsRoad(clickedHex);
            switch (towerConfig.RegularParameters.PlacementType)
            {
                case TowerPlacementType.Anywhere:
                    break;
                case TowerPlacementType.NotRoad:
                    if (isRoad)
                    {
                        return;
                    }
                    break;
                case TowerPlacementType.OnRoad:
                    if (!isRoad)
                    {
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value >= towerConfig.TowerLevelConfigs[0].BuildPrice)
                _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedHex, 
                    new TowerShortParams(_context.PlayerHandController.ChosenTowerType, 1));
        }

        private void ProcessBuild(Hex2d position, TowerShortParams towerShortParams)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Free))
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.PlayerHandController.UseChosenTower(towerConfig.TowerLevelConfigs[0].BuildPrice);
            _context.ConstructionProcessController.SetTowerBuilding(towerConfig, position);
        }

        private void ProcessPreManipulation(Hex2d clickedHex)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(clickedHex, FieldHexType.Tower))
                return;
            
            int towerKey = clickedHex.GetHashCode();

            if (!_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(towerKey,
                    out TowerController towerInstance))
                return;

            if (!towerInstance.CanShoot)
                return;
            
            TowerShortParams towerShortParams = towerInstance.GetShortParams();
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            
            _context.TowerManipulationWindowController.ShowWindow(towerConfig, towerShortParams.Level,
                _context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value,
                () =>
                {
                    if (_context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value >= towerConfig.TowerLevelConfigs[towerShortParams.Level].BuildPrice)
                        _context.MatchCommands.Outgoing.RequestUpgradeTower.Fire(clickedHex, towerShortParams);
                },
                () =>
                {
                    towerInstance.ShowSelection();
                    _context.TowerInfoWindowController.ShowWindow(towerConfig, towerShortParams.Level,
                        //towerInstance.Buffs,
                        () =>
                        {
                            towerInstance.HideSelection();
                            _context.TowerManipulationWindowController.ShowWindow();
                        });
                },
                () => _context.MatchCommands.Outgoing.RequestSellTower.Fire(clickedHex, towerShortParams));
        }

        private void ProcessUpgrade(Hex2d position, TowerShortParams towerShortParams)
        {
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Tower))
                return;
            
            int positionHashcode = position.GetHashCode();
            
            if (!_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(positionHashcode,
                    out TowerController towerInstance))
                return;

            if (!towerInstance.CanShoot)
                return;

            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.PlayerHandController.UseChosenTower(towerConfig.TowerLevelConfigs[towerShortParams.Level].BuildPrice);
            _context.ConstructionProcessController.SetTowerUpgrading(towerInstance);
        }

        private void ProcessSell(Hex2d position, TowerShortParams towerShortParams)
        {
            int positionHashcode = position.GetHashCode();
            
            if (!_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(positionHashcode,
                    out TowerController towerInstance))
                return;
            
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Tower)
                || !towerInstance.CanShoot)
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            int sellPrice = TowerController.GetTowerSellPrice(towerConfig.TowerLevelConfigs, towerShortParams.Level);
            _context.PlayerHandController.AddEnergy(sellPrice);
            _context.ConstructionProcessController.SetTowerRemoving(position);
        }
    }
}