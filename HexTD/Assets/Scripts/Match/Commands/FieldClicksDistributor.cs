using HexSystem;
using Match.Field;
using Match.Field.Currency;
using Match.Field.Shooting;
using Match.Field.Tower;
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
            public ShootingController ShootingController { get; }
            public CurrencyController CurrencyController { get; }
            public MatchCommands MatchCommands { get; }

            public Context(
                FieldModel fieldModel, FieldClicksHandler clicksHandler, ConfigsRetriever configsRetriever,
                FieldConstructionProcessController constructionProcessController,
                ShootingController shootingController,
                CurrencyController currencyController,
                MatchCommands matchCommands)
            {
                FieldModel = fieldModel;
                ClicksHandler = clicksHandler;
                ConfigsRetriever = configsRetriever;
                ConstructionProcessController = constructionProcessController;
                ShootingController = shootingController;
                CurrencyController = currencyController;
                MatchCommands = matchCommands;
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
                    ProcessBuild(clickedCell, new TowerShortParams(TowerType._001, 1));
                    break;
                
                case FieldHexType.Tower:
                    ProcessPreManipulation(clickedCell);
                    break;
            }
        }

        private void ProcessPreBuild(Hex2d clickedCell)
        {
            //_context.TowerSelectionWindowController.ShowWindow(_context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
            //    (towerToBuild) =>
            //{
            //    TowerConfig towerConfig = _context.ConfigsRetriever.GetTowerByType(towerToBuild.TowerType);
            //    
            //    if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.Parameters.Levels[0].LevelRegularParams.Data.Price)
            //        _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedCell, towerToBuild);
            //});
        }

        private void ProcessBuild(Hex2d position, TowerShortParams towerShortParams)
        {
            // check consistency
            //if (_context.FieldModel.Cells[position.y, position.x] != FieldCellType.Free)
            //    return;
            
            TowerConfig towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            
            _context.CurrencyController.SpendSilver(towerConfig.Parameters.Levels[0].LevelRegularParams.Data.Price);
            
            TowerController towerInstance = _context.ConstructionProcessController.SetTowerBuilding(towerConfig, position);

            // try to target new tower to blocker
            if (_context.ShootingController.BlockerTargetId != -1)
                towerInstance.SetBlockerTarget(_context.ShootingController.BlockerTargetId);
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
            TowerConfig towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            
            //_context.TowerManipulationWindowController.ShowWindow(towerConfig.Parameters, towerShortParams.Level,
            //    _context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
            //    () =>
            //    {
            //        if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.Parameters.Levels[towerShortParams.Level].LevelRegularParams.Data.Price)
            //            _context.MatchCommands.Outgoing.RequestUpgradeTower.Fire(clickedHex, towerShortParams);
            //    },
            //    () =>
            //    {
            //        towerInstance.ShowSelection();
            //        _context.TowerInfoWindowController.ShowWindow(towerConfig.Parameters, towerShortParams.Level,
            //            //towerInstance.Buffs,
            //            () =>
            //            {
            //                towerInstance.HideSelection();
            //                _context.TowerManipulationWindowController.ShowWindow();
            //            });
            //    },
            //    () => _context.MatchCommands.Outgoing.RequestSellTower.Fire(clickedHex, towerShortParams));
        }

        private void ProcessUpgrade(Hex2d position, TowerShortParams towerShortParams)
        {
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Tower))
                return;
            
            TowerController towerInstance = _context.FieldModel.TowersByPositions[position.GetHashCode()];

            if (!towerInstance.CanShoot)
                return;

            TowerConfig towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.CurrencyController.SpendSilver(towerConfig.Parameters.Levels[towerShortParams.Level].LevelRegularParams.Data.Price);
            _context.ConstructionProcessController.SetTowerUpgrading(towerInstance);
        }

        private void ProcessSell(Hex2d position, TowerShortParams towerShortParams)
        {
            int positionHashcode = position.GetHashCode();
            
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Tower)
                || !_context.FieldModel.TowersByPositions[positionHashcode].CanShoot)
                return;
            
            TowerConfig towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            int sellPrice = TowerController.GetTowerSellPrice(towerConfig.Parameters, towerShortParams.Level);
            _context.CurrencyController.AddSilver(sellPrice);
            _context.ConstructionProcessController.SetTowerRemoving(position);
        }
    }
}