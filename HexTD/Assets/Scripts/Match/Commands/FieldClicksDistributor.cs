using Match.Field;
using Match.Field.Currency;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Windows;
using Match.Windows.Tower;
using Tools;
using Tools.Interfaces;
using UnityEngine;

namespace Match.Commands
{
    public class FieldClicksDistributor : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public int FieldWidth { get; }
            public int FieldHeight { get; }
            public FieldModel FieldModel { get; }
            public FieldClicksHandler ClicksHandler { get; }
            public TowerConfigRetriever TowerConfigRetriever { get; }
            public FieldConstructionProcessController ConstructionProcessController { get; }
            public ShootingController ShootingController { get; }
            public CurrencyController CurrencyController { get; }
            public MatchCommands MatchCommands { get; }
            
            public MatchInfoPanelController MatchInfoPanelController { get; }
            public TowerSelectionWindowController TowerSelectionWindowController { get; }
            public TowerManipulationWindowController TowerManipulationWindowController { get; }
            public TowerInfoWindowController TowerInfoWindowController { get; }

            public Context(int fieldWidth, int fieldHeight,
                FieldModel fieldModel, FieldClicksHandler clicksHandler, TowerConfigRetriever towerConfigRetriever,
                FieldConstructionProcessController constructionProcessController,
                ShootingController shootingController,
                CurrencyController currencyController,
                MatchCommands matchCommands,
                
                MatchInfoPanelController matchInfoPanelController,
                TowerSelectionWindowController towerSelectionWindowController,
                TowerManipulationWindowController towerManipulationWindowController,
                TowerInfoWindowController towerInfoWindowController)
            {
                FieldWidth = fieldWidth;
                FieldHeight = fieldHeight;
                FieldModel = fieldModel;
                ClicksHandler = clicksHandler;
                TowerConfigRetriever = towerConfigRetriever;
                ConstructionProcessController = constructionProcessController;
                ShootingController = shootingController;
                CurrencyController = currencyController;
                MatchCommands = matchCommands;

                MatchInfoPanelController = matchInfoPanelController;
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
                Vector2Int clickedCell = _context.ClicksHandler.GetClickedCell();

                if (!_context.ClicksHandler.IsClickValuable(clickedCell))
                    return;

                DistributeClick(clickedCell);
            }
        }

        private void DistributeClick(Vector2Int clickedCell)
        {
            switch (_context.FieldModel.Cells[clickedCell.y, clickedCell.x])
            {
                case FieldCellType.Free:
                    ProcessPreBuild(clickedCell);
                    break;
                
                case FieldCellType.Tower:
                    ProcessPreManipulation(clickedCell);
                    break;
            }
        }

        private void ProcessPreBuild(Vector2Int clickedCell)
        {
            _context.TowerSelectionWindowController.ShowWindow(_context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
                (towerToBuild) =>
            {
                TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerToBuild.TowerType);
                
                if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.Parameters.Levels[0].LevelRegularParams.Data.Price)
                    _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedCell, towerToBuild);
            });
        }

        private void ProcessBuild(Vector2Int position, TowerShortParams towerShortParams)
        {
            // check consistency
            if (_context.FieldModel.Cells[position.y, position.x] != FieldCellType.Free)
                return;
            
            TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerShortParams.TowerType);
            
            _context.CurrencyController.SpendSilver(towerConfig.Parameters.Levels[0].LevelRegularParams.Data.Price);
            
            TowerController towerInstance = _context.ConstructionProcessController.SetTowerBuilding(towerConfig, position);

            // try to target new tower to blocker
            if (_context.ShootingController.BlockerTargetId != -1)
                towerInstance.SetBlockerTarget(_context.ShootingController.BlockerTargetId);
        }

        private void ProcessPreManipulation(Vector2Int clickedCell)
        {
            // check consistency
            if (_context.FieldModel.Cells[clickedCell.y, clickedCell.x] != FieldCellType.Tower)
                return;
            
            int towerKey = clickedCell.GetHashCode(_context.FieldWidth);
            TowerController towerInstance = _context.FieldModel.TowersByPositions[towerKey];

            if (!towerInstance.CanShoot)
                return;
            
            TowerShortParams towerShortParams = towerInstance.GetShortParams();
            TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerShortParams.TowerType);
            
            _context.TowerManipulationWindowController.ShowWindow(towerConfig.Parameters, towerShortParams.Level,
                _context.CurrencyController.GoldCoinsCountReactiveProperty.Value,
                () =>
                {
                    if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value >= towerConfig.Parameters.Levels[towerShortParams.Level].LevelRegularParams.Data.Price)
                        _context.MatchCommands.Outgoing.RequestUpgradeTower.Fire(clickedCell, towerShortParams);
                },
                () =>
                {
                    towerInstance.ShowSelection();
                    _context.TowerInfoWindowController.ShowWindow(towerConfig.Parameters, towerShortParams.Level,
                        //towerInstance.Buffs,
                        () =>
                        {
                            towerInstance.HideSelection();
                            _context.TowerManipulationWindowController.ShowWindow();
                        });
                },
                () => _context.MatchCommands.Outgoing.RequestSellTower.Fire(clickedCell, towerShortParams));
        }

        private void ProcessUpgrade(Vector2Int position, TowerShortParams towerShortParams)
        {
            // check consistency
            int towerKey = position.GetHashCode(_context.FieldWidth);
            
            if (_context.FieldModel.Cells[position.y, position.x] != FieldCellType.Tower)
                return;
            
            TowerController towerInstance = _context.FieldModel.TowersByPositions[towerKey];

            if (!towerInstance.CanShoot)
                return;

            TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.CurrencyController.SpendSilver(towerConfig.Parameters.Levels[towerShortParams.Level].LevelRegularParams.Data.Price);
            _context.ConstructionProcessController.SetTowerUpgrading(towerInstance);
        }

        private void ProcessSell(Vector2Int position, TowerShortParams towerShortParams)
        {
            int positionHashcode = position.GetHashCode(_context.FieldWidth);
            
            if (_context.FieldModel.Cells[position.y, position.x] != FieldCellType.Tower
                || !_context.FieldModel.TowersByPositions[positionHashcode].CanShoot)
                return;
            
            TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerShortParams.TowerType);
            int sellPrice = TowerController.GetTowerSellPrice(towerConfig.Parameters, towerShortParams.Level);
            _context.CurrencyController.AddSilver(sellPrice);
            _context.ConstructionProcessController.SetTowerRemoving(position);
        }
    }
}