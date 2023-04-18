using System;
using System.Collections.Generic;
using Match.Field.Hand;
using Match.Field.Tower;
using Services;
using UniRx;
using Object = UnityEngine.Object;

namespace Match.Windows.Tower
{
    public class TowerSelectionWindowController : BaseWindowController
    {
        public struct Context
        {
            public TowerSelectionWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public PlayerHandController PlayerHandController { get; }
            public ReactiveCommand<int> CoinsCountChangedReactiveCommand { get; }

            public Context(TowerSelectionWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty,
                ConfigsRetriever configsRetriever, PlayerHandController playerHandController,
                ReactiveCommand<int> coinsCountChangedReactiveCommand)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
                ConfigsRetriever = configsRetriever;
                PlayerHandController = playerHandController;
                CoinsCountChangedReactiveCommand = coinsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly List<TowerItemView> _playerHandTowerItems;
        private Action<TowerShortParams> _onTowerSelectedAction;

        public TowerSelectionWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;

            _context.View.CloseButton.onClick.AddListener(HideWindow);
            // artifact towerItems can be skipped here, as their price is 0
            _context.CoinsCountChangedReactiveCommand.Subscribe(UpdatePlayerHandTowers);
            _playerHandTowerItems = new List<TowerItemView>(_context.PlayerHandController.Towers.Count);
            FillContent();
        }

        private void FillContent()
        {
            foreach (TowerType tower in _context.PlayerHandController.Towers)
            {
                AddPlayerHandTower(tower);
            }
        }

        private void AddPlayerHandTower(TowerType towerType)
        {
            if (towerType == TowerType.Undefined)
                return;
                
            TowerItemView towerItem = Object.Instantiate(_context.View.TowerItemPrefab, _context.View.ContentRoot);
            towerItem.Init(_context.ConfigsRetriever.GetTowerByType(towerType),
                () => OnTowerSelectedHandler(towerItem.TowerShortParams, false));
            
            _playerHandTowerItems.Add(towerItem);
        }

        private void OnTowerSelectedHandler(TowerShortParams towerShortParams, bool isFree)
        {
            _onTowerSelectedAction.Invoke(towerShortParams);
            HideWindow();
        }
        
        public void ShowWindow(int currentCoinsCount, Action<TowerShortParams> onTowerSelectedAction)
        {
            // update select action
            _onTowerSelectedAction = onTowerSelectedAction;

            // refresh towers from player hand
            UpdatePlayerHandTowers(currentCoinsCount);
            
            base.ShowWindow();
        }

        private void UpdatePlayerHandTowers(int coinsCount)
        {
            foreach (TowerItemView towerItem in _playerHandTowerItems)
            {
                towerItem.Refresh(coinsCount);
            }
        }

        protected override void OnDispose()
        {
            _playerHandTowerItems.Clear();
            base.OnDispose();
            
            _context.View.CloseButton.onClick.RemoveListener(HideWindow);
        }
    }
}