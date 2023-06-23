using System;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using UniRx;

namespace Match.Windows.Tower
{
    public class TowerManipulationWindowController : BaseWindowController
    {
        public struct Context
        {
            public TowerManipulationWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }
            public ReactiveCommand<int> CoinsCountChangedReactiveCommand { get; }
            
            public Context(TowerManipulationWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty,
                ReactiveCommand<int> coinsCountChangedReactiveCommand)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
                CoinsCountChangedReactiveCommand = coinsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        
        private readonly TowerInfoPanelController _towerInfoPanelController;
        private bool _hasUpgrade;
        private int _currentUpgradePrice;
        private int _currentSellPrice;
        private Action _onTowerUpgradeClickAction;
        private Action _onTowerSellClickAction;
        private Action _onCloseClickAction;
        
        public TowerManipulationWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;

            TowerInfoPanelController.Context towerInfoPanelControllerContext =
                new TowerInfoPanelController.Context(_context.View.TowerInfoPanel);
            _towerInfoPanelController = AddDisposable(new TowerInfoPanelController(towerInfoPanelControllerContext));
            
            _context.View.CloseButton.onClick.AddListener(CloseWindow);
            _context.View.UpgradeButton.onClick.AddListener(OnTowerUpgradeClickedHandler);
            _context.View.SellButton.onClick.AddListener(OnTowerSellClickedHandler);

            _context.CoinsCountChangedReactiveCommand.Subscribe(Refresh);
        }
        
        public void ShowWindow(TowerConfigNew towerParameters, int towerLevel, int currentCoinsCount,
            Action onTowerUpgradeClickAction,
            Action onTowerSellClickAction,
            Action onCloseClickAction)
        {
            _onTowerUpgradeClickAction = onTowerUpgradeClickAction;
            _onTowerSellClickAction = onTowerSellClickAction;
            _onCloseClickAction = onCloseClickAction;
            
            _towerInfoPanelController.Init(towerParameters, towerLevel);

            _hasUpgrade = false;//towerLevel < towerParameters.TowerLevelConfigs.Count;
            _currentUpgradePrice = _hasUpgrade ? towerParameters.TowerLevelConfigs[towerLevel + 1].BuildPrice : -1;
            _currentSellPrice = TowerController.GetTowerSellPrice(towerParameters.TowerLevelConfigs, towerLevel);
            
            Refresh(currentCoinsCount);

            base.ShowWindow();
        }

        private void Refresh(int coinsCount)
        {
            if (_hasUpgrade)
            {
                _context.View.UpgradePriceText.text = $"{_currentUpgradePrice}";
                _context.View.UpgradeButton.gameObject.SetActive(true);
                _context.View.UpgradeButton.interactable = _currentUpgradePrice <= coinsCount;
            }
            else
                _context.View.UpgradeButton.gameObject.SetActive(false);

            _context.View.SellPriceText.text = $"{_currentSellPrice}";
        }

        private void OnTowerUpgradeClickedHandler()
        {
            _onTowerUpgradeClickAction.Invoke();
            CloseWindow();
        }
        
        private void OnTowerSellClickedHandler()
        {
            _onTowerSellClickAction.Invoke();
            CloseWindow();
        }

        private void CloseWindow()
        {
            HideWindow();
            _towerInfoPanelController.Clear();
            _onCloseClickAction.Invoke();
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            
            _context.View.CloseButton.onClick.RemoveListener(HideWindow);
        }
    }
}