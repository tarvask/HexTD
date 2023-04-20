using Cysharp.Threading.Tasks;
using Match.Field.Castle;
using Match.Field.Hand;
using Match.Wave;
using Match.Windows;
using Match.Windows.Hand;
using Match.Windows.Tower;
using Services;
using Tools;
using Tools.Interfaces;
using UI.MatchInfoWindow;
using UniRx;
using UnityEngine;

namespace Match
{
    public class WindowsManager : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public MatchUiViewsCollection UiViews { get; }
            public Canvas Canvas { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public PlayerHandController PlayerHandController { get; }
            public WaveParams[] Waves { get; }
            
            public IReadOnlyReactiveProperty<bool> IsConnectedReactiveProperty { get; }
            public ReactiveCommand<HealthInfo> EnemyCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<HealthInfo> OurCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<float> MatchStartedReactiveCommand { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public ReactiveCommand<float> BetweenWavesPlanningStartedReactiveCommand { get; }
            public ReactiveCommand<float> ArtifactChoosingStartedReactiveCommand { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurGoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurCrystalsCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<bool> DragCardChangeStatusCommand { get; }
            public ReactiveCommand QuitMatchReactiveCommand { get; }
            public WindowSystem.IWindowsManager NewWindowsManager { get; }

            public Context(MatchUiViewsCollection uiViews, Canvas canvas,
                ConfigsRetriever configsRetriever,
                PlayerHandController playerHandController,
                WaveParams[] waves,

                IReadOnlyReactiveProperty<bool> isConnectedReactiveProperty,
                ReactiveCommand<HealthInfo> enemyCastleHealthChangedReactiveCommand,
                ReactiveCommand<HealthInfo> ourCastleHealthChangedReactiveCommand,
                ReactiveCommand<float> matchStartedReactiveCommand,
                ReactiveCommand<float> waveStartedReactiveCommand,
                ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand,
                ReactiveCommand<float> artifactChoosingStartedReactiveCommand,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand<int> ourGoldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> ourCrystalsCoinsCountChangedReactiveCommand,
                ReactiveCommand<bool> dragCardChangeStatusCommand,
                ReactiveCommand quitMatchReactiveCommand,
                WindowSystem.IWindowsManager newWindowsManager)
            {
                UiViews = uiViews;
                Canvas = canvas;
                ConfigsRetriever = configsRetriever;
                PlayerHandController = playerHandController;
                Waves = waves;

                IsConnectedReactiveProperty = isConnectedReactiveProperty;
                EnemyCastleHealthChangedReactiveCommand = enemyCastleHealthChangedReactiveCommand;
                OurCastleHealthChangedReactiveCommand = ourCastleHealthChangedReactiveCommand;
                MatchStartedReactiveCommand = matchStartedReactiveCommand;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                BetweenWavesPlanningStartedReactiveCommand = betweenWavesPlanningStartedReactiveCommand;
                ArtifactChoosingStartedReactiveCommand = artifactChoosingStartedReactiveCommand;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                OurGoldenCoinsCountChangedReactiveCommand = ourGoldenCoinsCountChangedReactiveCommand;
                OurCrystalsCoinsCountChangedReactiveCommand = ourCrystalsCoinsCountChangedReactiveCommand;
                QuitMatchReactiveCommand = quitMatchReactiveCommand;
                NewWindowsManager = newWindowsManager;
                DragCardChangeStatusCommand = dragCardChangeStatusCommand;
            }
        }

        private readonly Context _context;

        // hud
        private readonly MatchInfoPanelController _matchInfoPanelController;
        private readonly UniTaskCompletionSource<MatchInfoWindowController> _matchInfoWindowController = 
            new UniTaskCompletionSource<MatchInfoWindowController>();
        
        // windows
        private readonly MatchStartInfoWindowController _matchStartInfoWindowController;
        private readonly HandTowerSelectionController _handTowerSelectionController;
        private readonly WaveStartInfoWindowController _waveStartInfoWindowController;
        private readonly WinLoseWindowController _winLoseWindowController;
        private readonly DisconnectBlockerWindowController _disconnectBlockerWindowController;
        
        private readonly TowerSelectionWindowController _towerSelectionWindowController;
        private readonly TowerManipulationWindowController _towerManipulationWindowController;
        private readonly TowerInfoWindowController _towerInfoWindowController;
        private readonly MobInfoWindowController _mobInfoWindowController;
        
        public readonly ReactiveProperty<int> OpenWindowsCountReactiveProperty;

        public MatchInfoPanelController MatchInfoPanelController => _matchInfoPanelController;
        public WinLoseWindowController WinLoseWindowController => _winLoseWindowController;
        public TowerSelectionWindowController TowerSelectionWindowController => _towerSelectionWindowController;
        public TowerManipulationWindowController TowerManipulationWindowController => _towerManipulationWindowController;
        public TowerInfoWindowController TowerInfoWindowController => _towerInfoWindowController;
        public MobInfoWindowController MobInfoWindowController => _mobInfoWindowController;

        public WindowsManager(Context context)
        {
            _context = context;
            
            OpenWindowsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>());
            
            // match start info
            MatchStartInfoWindowController.Context matchStartInfoWindowControllerContext = new MatchStartInfoWindowController.Context(
                _context.UiViews.MatchStartInfoView, OpenWindowsCountReactiveProperty);
            _matchStartInfoWindowController = AddDisposable(new MatchStartInfoWindowController(matchStartInfoWindowControllerContext));
            
            // tower selection hud from hand
            HandTowerSelectionController.Context handTowerSelectionControllerContext = new HandTowerSelectionController.Context(
                _context.UiViews.HandTowerSelectionView, _context.PlayerHandController,
                _context.ConfigsRetriever, _context.DragCardChangeStatusCommand);
            _handTowerSelectionController = new HandTowerSelectionController(handTowerSelectionControllerContext);

            // wave start info
            WaveStartInfoWindowController.Context waveStartInfoWindowControllerContext = new WaveStartInfoWindowController.Context(
                _context.UiViews.WaveStartInfoWindowView, OpenWindowsCountReactiveProperty);
            _waveStartInfoWindowController = AddDisposable(new WaveStartInfoWindowController(waveStartInfoWindowControllerContext));
            
            // hud
            MatchInfoPanelController.Context matchInfoPanelControllerContext = new MatchInfoPanelController.Context(
                _context.UiViews.MatchInfoPanelView,
                _context.Canvas,
                _context.Waves,
                
                _context.EnemyCastleHealthChangedReactiveCommand,
                _context.OurCastleHealthChangedReactiveCommand,
                _context.WaveStartedReactiveCommand,
                _context.BetweenWavesPlanningStartedReactiveCommand,
                _context.WaveNumberChangedReactiveCommand,
                _context.OurGoldenCoinsCountChangedReactiveCommand,
                _context.OurCrystalsCoinsCountChangedReactiveCommand);
            _matchInfoPanelController = AddDisposable(new MatchInfoPanelController(matchInfoPanelControllerContext));

            // win/lose window
            WinLoseWindowController.Context winLoseWindowControllerContext = new WinLoseWindowController.Context(
                _context.UiViews.WinLoseWindowView, OpenWindowsCountReactiveProperty, _context.QuitMatchReactiveCommand);
            _winLoseWindowController = AddDisposable(new WinLoseWindowController(winLoseWindowControllerContext));
            
            // disconnect blocker window
            DisconnectBlockerWindowController.Context disconnectBlockerWindowControllerContext = new DisconnectBlockerWindowController.Context(
                _context.UiViews.DisconnectBlockerWindowView, OpenWindowsCountReactiveProperty);
            _disconnectBlockerWindowController = AddDisposable(new DisconnectBlockerWindowController(disconnectBlockerWindowControllerContext));
            
            // tower selection window
            TowerSelectionWindowController.Context towerSelectionWindowControllerContext = new TowerSelectionWindowController.Context(
                _context.UiViews.TowerSelectionWindowView, OpenWindowsCountReactiveProperty,
                _context.ConfigsRetriever, _context.PlayerHandController, _context.OurGoldenCoinsCountChangedReactiveCommand);
            _towerSelectionWindowController = AddDisposable(new TowerSelectionWindowController(towerSelectionWindowControllerContext));
            
            // tower manipulation window
            TowerManipulationWindowController.Context towerManipulationWindowControllerContext = new TowerManipulationWindowController.Context(
                _context.UiViews.TowerManipulationWindowView, OpenWindowsCountReactiveProperty, _context.OurGoldenCoinsCountChangedReactiveCommand);
            _towerManipulationWindowController = AddDisposable(new TowerManipulationWindowController(towerManipulationWindowControllerContext));
            
            // tower info window
            TowerInfoWindowController.Context towerInfoWindowControllerContext = new TowerInfoWindowController.Context(
                _context.UiViews.TowerInfoWindowView, OpenWindowsCountReactiveProperty);
            _towerInfoWindowController = AddDisposable(new TowerInfoWindowController(towerInfoWindowControllerContext));

            // mob info window
            MobInfoWindowController.Context mobInfoWindowControllerContext = new MobInfoWindowController.Context(
                _context.UiViews.MobInfoWindowView, OpenWindowsCountReactiveProperty);
            _mobInfoWindowController = AddDisposable(new MobInfoWindowController(mobInfoWindowControllerContext));

            // subscriptions
            _context.IsConnectedReactiveProperty.Subscribe(ConnectionStateChanged);
            _context.MatchStartedReactiveCommand.Subscribe((showDuration) =>
                _matchStartInfoWindowController.ShowWindow(showDuration));
            _context.WaveStartedReactiveCommand.Subscribe((showDuration) =>
                _waveStartInfoWindowController.ShowWindow(showDuration));

            InitAsync();
        }

        private async void InitAsync()
        {
//            var matchInfoWindowController = await _context.NewWindowsManager.OpenAsync<MatchInfoWindowController>();
//            _matchInfoWindowController.TrySetResult(matchInfoWindowController);
        } 

        public void OuterLogicUpdate(float frameLength)
        {
            _matchInfoPanelController.OuterLogicUpdate(frameLength);
            
            if (_matchStartInfoWindowController.IsShown)
                _matchStartInfoWindowController.OuterLogicUpdate(frameLength);
            
            if (_waveStartInfoWindowController.IsShown)
                _waveStartInfoWindowController.OuterLogicUpdate(frameLength);
        }

        private void ConnectionStateChanged(bool isConnected)
        {
            if (isConnected)
            {
                if (_disconnectBlockerWindowController.IsShown)
                    _disconnectBlockerWindowController.Hide();
            }
            else
            {
                if (!_disconnectBlockerWindowController.IsShown)
                    _disconnectBlockerWindowController.ShowWindow();
            }
        }
    }
}