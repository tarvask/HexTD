using Match.Field.Castle;
using Match.Field.Mob;
using Match.Field.Tower;
using Match.Wave;
using Match.Windows;
using Match.Windows.Tower;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match
{
    public class WindowsManager : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public MatchUiViewsCollection UiViews { get; }
            public Camera MainCamera { get; }
            public Canvas Canvas { get; }
            public TowerConfigRetriever TowerConfigRetriever { get; }
            public MobConfigRetriever MobConfigRetriever { get; }
            public PlayerHandParams PlayerHandParams { get; }
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
            public ReactiveCommand<int> OurGoldCoinsIncomeChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurCrystalsCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand QuitMatchReactiveCommand { get; }

            public Context(MatchUiViewsCollection uiViews, Camera mainCamera, Canvas canvas,
                TowerConfigRetriever towerConfigRetriever, MobConfigRetriever mobConfigRetriever,
                PlayerHandParams playerHandParams,
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
                ReactiveCommand<int> ourGoldCoinsIncomeChangedReactiveCommand,
                ReactiveCommand<int> ourCrystalsCoinsCountChangedReactiveCommand,
                ReactiveCommand quitMatchReactiveCommand)
            {
                UiViews = uiViews;
                MainCamera = mainCamera;
                Canvas = canvas;
                TowerConfigRetriever = towerConfigRetriever;
                MobConfigRetriever = mobConfigRetriever;
                PlayerHandParams = playerHandParams;
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
                OurGoldCoinsIncomeChangedReactiveCommand = ourGoldCoinsIncomeChangedReactiveCommand;
                OurCrystalsCoinsCountChangedReactiveCommand = ourCrystalsCoinsCountChangedReactiveCommand;
                QuitMatchReactiveCommand = quitMatchReactiveCommand;
            }
        }

        private readonly Context _context;

        // hud
        private readonly MatchInfoPanelController _matchInfoPanelController;
        // windows
        private readonly MatchStartInfoWindowController _matchStartInfoWindowController;
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
            
            // wave start info
            WaveStartInfoWindowController.Context waveStartInfoWindowControllerContext = new WaveStartInfoWindowController.Context(
                _context.UiViews.WaveStartInfoWindowView, OpenWindowsCountReactiveProperty);
            _waveStartInfoWindowController = AddDisposable(new WaveStartInfoWindowController(waveStartInfoWindowControllerContext));
            
            // hud
            MatchInfoPanelController.Context matchInfoPanelControllerContext = new MatchInfoPanelController.Context(
                _context.UiViews.MatchInfoPanelView,
                _context.MainCamera,
                _context.Canvas,
                _context.Waves,
                
                _context.EnemyCastleHealthChangedReactiveCommand,
                _context.OurCastleHealthChangedReactiveCommand,
                _context.WaveStartedReactiveCommand,
                _context.BetweenWavesPlanningStartedReactiveCommand,
                _context.WaveNumberChangedReactiveCommand,
                _context.OurGoldenCoinsCountChangedReactiveCommand,
                _context.OurGoldCoinsIncomeChangedReactiveCommand,
                _context.OurCrystalsCoinsCountChangedReactiveCommand);
            _matchInfoPanelController = AddDisposable(new MatchInfoPanelController(matchInfoPanelControllerContext));
            
            // extract middle panel shift
            int middlePanelTopShift = _matchInfoPanelController.MatchInfoPanelTopShift;
            
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
                _context.TowerConfigRetriever, _context.PlayerHandParams, _context.OurGoldenCoinsCountChangedReactiveCommand);
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