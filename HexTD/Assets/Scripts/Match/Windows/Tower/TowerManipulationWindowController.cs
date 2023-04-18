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
            public ReactiveCommand<int> SilverCoinsCountChangedReactiveCommand { get; }
            
            public Context(TowerManipulationWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty,
                ReactiveCommand<int> silverCoinsCountChangedReactiveCommand)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
                SilverCoinsCountChangedReactiveCommand = silverCoinsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private bool _hasUpgrade;
        private int _currentUpgradePrice;
        private int _currentSellPrice;
        private Action _onTowerUpgradeClickAction;
        private Action _onTowerInfoClickAction;
        private Action _onTowerSellClickAction;
        
        public TowerManipulationWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;
            
            _context.View.CloseButton.onClick.AddListener(HideWindow);
            _context.View.UpgradeButton.onClick.AddListener(OnTowerUpgradeClickedHandler);
            _context.View.InfoButton.onClick.AddListener(OnTowerInfoClickedHandler);
            _context.View.SellButton.onClick.AddListener(OnTowerSellClickedHandler);

            _context.SilverCoinsCountChangedReactiveCommand.Subscribe(Refresh);
        }
        
        public void ShowWindow(TowerConfigNew towerParameters, int towerLevel, int currentSilverCoinsCount,
            Action onTowerUpgradeClickAction,
            Action onTowerInfoClickAction,
            Action onTowerSellClickAction)
        {
            _onTowerUpgradeClickAction = onTowerUpgradeClickAction;
            _onTowerInfoClickAction = onTowerInfoClickAction;
            _onTowerSellClickAction = onTowerSellClickAction;

            _hasUpgrade = towerLevel < towerParameters.TowerLevelConfigs.Count;
            _currentUpgradePrice = _hasUpgrade ? towerParameters.TowerLevelConfigs[towerLevel-1].BuildPrice : -1;
            _currentSellPrice = TowerController.GetTowerSellPrice(towerParameters.TowerLevelConfigs, towerLevel);
            
            Refresh(currentSilverCoinsCount);

            base.ShowWindow();
        }

        private void Refresh(int silverCoinsCount)
        {
            if (_hasUpgrade)
            {
                _context.View.UpgradePriceText.text = $"{_currentUpgradePrice}";
                _context.View.UpgradeButton.gameObject.SetActive(true);
                _context.View.UpgradeButton.interactable = _currentUpgradePrice <= silverCoinsCount;
            }
            else
                _context.View.UpgradeButton.gameObject.SetActive(false);

            _context.View.SellPriceText.text = $"{_currentSellPrice}";
        }

        private void OnTowerUpgradeClickedHandler()
        {
            _onTowerUpgradeClickAction.Invoke();
            HideWindow();
        }
        
        private void OnTowerInfoClickedHandler()
        {
            _onTowerInfoClickAction.Invoke();
            HideWindow();
        }
        
        private void OnTowerSellClickedHandler()
        {
            _onTowerSellClickAction.Invoke();
            HideWindow();
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            
            _context.View.CloseButton.onClick.RemoveListener(HideWindow);
        }
    }
}