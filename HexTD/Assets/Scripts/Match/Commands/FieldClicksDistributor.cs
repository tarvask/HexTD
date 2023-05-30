using HexSystem;
using Match.Field;
using Match.Field.Hand;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Match.Windows.Tower;
using Services;
using Tools;
using Tools.Interfaces;
using UniRx;

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
            public TowerPlacer TowerPlacer { get; }
            public MatchCommands MatchCommands { get; }
            
            public TowerManipulationWindowController TowerManipulationWindowController { get; }
            public TowerInfoWindowController TowerInfoWindowController { get; }
            
            public ReactiveCommand<Hex2d> PlaceForTowerSelectedCommand { get; }

            public Context(
                FieldModel fieldModel, FieldClicksHandler clicksHandler, 
                ConfigsRetriever configsRetriever,
                FieldConstructionProcessController constructionProcessController,
                PlayerHandController playerHandController,
                TowerPlacer towerPlacer,
                MatchCommands matchCommands,

                TowerManipulationWindowController towerManipulationWindowController,
                TowerInfoWindowController towerInfoWindowController,
                
                ReactiveCommand<Hex2d> placeForTowerSelectedCommand)
            {
                FieldModel = fieldModel;
                ClicksHandler = clicksHandler;
                ConfigsRetriever = configsRetriever;
                ConstructionProcessController = constructionProcessController;
                PlayerHandController = playerHandController;
                TowerPlacer = towerPlacer;
                MatchCommands = matchCommands;
                
                TowerManipulationWindowController = towerManipulationWindowController;
                TowerInfoWindowController = towerInfoWindowController;

                PlaceForTowerSelectedCommand = placeForTowerSelectedCommand;
            }
        }
        
        private readonly Context _context;

        public FieldClicksDistributor(Context context)
        {
            _context = context;
            
            _context.MatchCommands.Incoming.ApplyBuildTower.Subscribe(ProcessBuild);
            _context.MatchCommands.Incoming.ApplyUpgradeTower.Subscribe(ProcessUpgrade);
            _context.MatchCommands.Incoming.ApplySellTower.Subscribe(ProcessSell);

            _context.PlaceForTowerSelectedCommand.Subscribe(ProcessPreBuild);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            if (_context.ClicksHandler?.ClickedCellsCount > 0)
            {
                Hex2d clickedCell = _context.ClicksHandler.GetClickedCell();

                DistributeClick(clickedCell);
                return;
            }
            
            _context.TowerPlacer?.OuterLogicUpdate(frameLength);
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
            if (!_context.PlayerHandController.IsTowerChoice)
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(_context.PlayerHandController.ChosenTowerType);

            if (!_context.TowerPlacer.CanTowerBePlacedToHex(towerConfig, clickedHex))
                return;

            if (_context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value >=
                towerConfig.TowerLevelConfigs[TowerConfigNew.FirstTowerLevel].BuildPrice)
                _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedHex, 
                    new TowerShortParams(_context.PlayerHandController.ChosenTowerType, TowerConfigNew.FirstTowerLevel));
        }

        private void ProcessBuild(Hex2d position, TowerShortParams towerShortParams)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Free))
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.PlayerHandController.UseChosenTower(towerConfig.TowerLevelConfigs[TowerConfigNew.FirstTowerLevel].BuildPrice);
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
            towerInstance.ShowSelection();
            
            _context.TowerManipulationWindowController.ShowWindow(towerConfig, towerShortParams.Level,
                _context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value,
                () =>
                {
                    // check if we can pay for the next level 
                    if (_context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value >=
                        towerConfig.TowerLevelConfigs[towerShortParams.NextLevel].BuildPrice)
                        _context.MatchCommands.Outgoing.RequestUpgradeTower.Fire(clickedHex, towerShortParams);
                },
                () => _context.MatchCommands.Outgoing.RequestSellTower.Fire(clickedHex, towerShortParams),
                () => towerInstance.HideSelection());
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
            _context.PlayerHandController.UseChosenTower(towerConfig.TowerLevelConfigs[towerShortParams.NextLevel].BuildPrice);
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