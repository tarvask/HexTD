using HexSystem;
using Match.Field;
using Match.Field.Currency;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Match.Windows;
using Match.Windows.Tower;
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
            public CurrencyController CurrencyController { get; }
            public MatchCommands MatchCommands { get; }
            
            public TowerSelectionWindowController TowerSelectionWindowController { get; }
            public TowerManipulationWindowController TowerManipulationWindowController { get; }
            public TowerInfoWindowController TowerInfoWindowController { get; }

            public Context(
                FieldModel fieldModel, FieldClicksHandler clicksHandler, ConfigsRetriever configsRetriever,
                FieldConstructionProcessController constructionProcessController,
                CurrencyController currencyController,
                MatchCommands matchCommands,
                TowerSelectionWindowController towerSelectionWindowController,
                TowerManipulationWindowController towerManipulationWindowController,
                TowerInfoWindowController towerInfoWindowController)
            {
                FieldModel = fieldModel;
                ClicksHandler = clicksHandler;
                ConfigsRetriever = configsRetriever;
                ConstructionProcessController = constructionProcessController;
                CurrencyController = currencyController;
                MatchCommands = matchCommands;
                
                TowerSelectionWindowController = towerSelectionWindowController;
                TowerManipulationWindowController = towerManipulationWindowController;
                TowerInfoWindowController = towerInfoWindowController;
            }
        }

        private readonly Context _context;

        public FieldClicksDistributor(Context context)
        {
            _context = context;
            
            _context.MatchCommands.Incoming.ApplyBuildTower.Subscribe(ProcessBuild);
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
                    ProcessPreBuild(clickedCell);
                    break;
                
                case FieldHexType.Tower:
                    ProcessPreManipulation(clickedCell);
                    break;
            }
        }

        private void ProcessPreBuild(Hex2d clickedCell)
        {
            _context.TowerSelectionWindowController.ShowWindow(_context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
                (towerToBuild) =>
            {
                TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerToBuild.TowerType);
                
                if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.TowerLevelConfigs[0].BuildPrice)
                    _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedCell, towerToBuild);
            });
        }

        private void ProcessBuild(Hex2d position, TowerShortParams towerShortParams)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Free))
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.CurrencyController.SpendSilver(towerConfig.TowerLevelConfigs[0].BuildPrice);
            _context.ConstructionProcessController.SetTowerBuilding(towerConfig, position);
        }

        private void ProcessPreManipulation(Hex2d clickedHex)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(clickedHex, FieldHexType.Tower))
                return;
            
            int towerKey = clickedHex.GetHashCode();
            TowerController towerInstance = _context.FieldModel.TowersByPositions[towerKey];

            if (!towerInstance.CanShoot)
                return;
            
            TowerShortParams towerShortParams = towerInstance.GetShortParams();
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            
            _context.TowerManipulationWindowController.ShowWindow(towerConfig, towerShortParams.Level,
                _context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
                () =>
                {
                    if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.TowerLevelConfigs[towerShortParams.Level].BuildPrice)
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
            
            TowerController towerInstance = _context.FieldModel.TowersByPositions[position.GetHashCode()];

            if (!towerInstance.CanShoot)
                return;

            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.CurrencyController.SpendSilver(towerConfig.TowerLevelConfigs[towerShortParams.Level].BuildPrice);
            _context.ConstructionProcessController.SetTowerUpgrading(towerInstance);
        }

        private void ProcessSell(Hex2d position, TowerShortParams towerShortParams)
        {
            int positionHashcode = position.GetHashCode();
            
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Tower)
                || !_context.FieldModel.TowersByPositions[positionHashcode].CanShoot)
                return;
            
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            int sellPrice = TowerController.GetTowerSellPrice(towerConfig.TowerLevelConfigs, towerShortParams.Level);
            _context.CurrencyController.AddSilver(sellPrice);
            _context.ConstructionProcessController.SetTowerRemoving(position);
        }
    }
}