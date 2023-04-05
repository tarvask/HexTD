using System;
using System.Collections.Generic;
using Match.Field.Tower;
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
            public TowerConfigRetriever TowerConfigRetriever { get; }
            public PlayerHandParams PlayerHandParams { get; }
            public ReactiveCommand<int> SilverCoinsCountChangedReactiveCommand { get; }

            public Context(TowerSelectionWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty,
                TowerConfigRetriever towerConfigRetriever, PlayerHandParams playerHandParams,
                ReactiveCommand<int> silverCoinsCountChangedReactiveCommand)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
                TowerConfigRetriever = towerConfigRetriever;
                PlayerHandParams = playerHandParams;
                SilverCoinsCountChangedReactiveCommand = silverCoinsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly List<TowerItemView> _playerHandTowerItems;
        private readonly List<TowerItemView> _artifactTowerItems;
        private Action<TowerShortParams> _onTowerSelectedAction;

        public TowerSelectionWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;

            _context.View.CloseButton.onClick.AddListener(HideWindow);
            // artifact towerItems can be skipped here, as their price is 0
            _context.SilverCoinsCountChangedReactiveCommand.Subscribe(UpdatePlayerHandTowers);
            _playerHandTowerItems = new List<TowerItemView>(_context.PlayerHandParams.Towers.Length);
            // we don't know count beforehand, hand size is ok
            _artifactTowerItems = new List<TowerItemView>(_context.PlayerHandParams.Towers.Length); 
            FillContent();
        }

        private void FillContent()
        {
            foreach (TowerType tower in _context.PlayerHandParams.Towers)
            {
                AddPlayerHandTower(tower);
            }
        }

        private void AddPlayerHandTower(TowerType towerType)
        {
            if (towerType == TowerType.Undefined)
                return;
                
            TowerItemView towerItem = Object.Instantiate(_context.View.TowerItemPrefab, _context.View.ContentRoot);
            towerItem.Init(_context.TowerConfigRetriever.GetTowerByType(towerType),
                () => OnTowerSelectedHandler(towerItem.TowerShortParams, false));
            
            _playerHandTowerItems.Add(towerItem);
        }

        private void AddArtifactTower(TowerType towerType)
        {
            if (towerType == TowerType.Undefined)
                return;
                
            TowerItemView towerItem = Object.Instantiate(_context.View.TowerItemPrefab, _context.View.ContentRoot);
            towerItem.Init(_context.TowerConfigRetriever.GetTowerByType(towerType),
                0,
                () => OnTowerSelectedHandler(towerItem.TowerShortParams, true));

            _artifactTowerItems.Add(towerItem);
        }

        private void OnTowerSelectedHandler(TowerShortParams towerShortParams, bool isFree)
        {
            _onTowerSelectedAction.Invoke(towerShortParams);
            HideWindow();
        }
        
        public void ShowWindow(int currentSilverCoinsCount, Action<TowerShortParams> onTowerSelectedAction)
        {
            // update select action
            _onTowerSelectedAction = onTowerSelectedAction;

            // refresh towers from player hand
            UpdatePlayerHandTowers(currentSilverCoinsCount);
            
            base.ShowWindow();
        }

        private void UpdatePlayerHandTowers(int silverCoinsCount)
        {
            foreach (TowerItemView towerItem in _playerHandTowerItems)
            {
                towerItem.Refresh(silverCoinsCount);
            }
        }

        protected override void OnDispose()
        {
            _playerHandTowerItems.Clear();
            _artifactTowerItems.Clear();
            base.OnDispose();
            
            _context.View.CloseButton.onClick.RemoveListener(HideWindow);
        }
    }
}