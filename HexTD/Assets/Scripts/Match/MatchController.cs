using System;
using HexSystem;
using MapEditor;
using Match.Commands;
using Match.Field;
using Match.Field.Castle;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.State;
using Match.Field.Tower;
using Match.State;
using Match.Wave;
using PathSystem;
using Services;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match
{
    public class MatchController : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public MatchView MatchView { get; }
            public MatchInitDataParameters MatchInitDataParameters { get; }
            public FieldConfig FieldConfig { get; }
            public MatchCommands MatchCommandsEnemy { get; }
            public MatchCommands MatchCommandsOur { get; }
            public MatchCommonCommands MatchCommandsCommon { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }
            public ReactiveCommand QuitMatchReactiveCommand { get; }
            public ReactiveCommand<int> SyncFrameCounterReactiveCommand { get; }
            public IReadOnlyReactiveProperty<ProcessRoles> OurGameRoleReactiveProperty { get; }
            public IReadOnlyReactiveProperty<NetworkRoles> OurNetworkRoleReactiveProperty { get; }
            public IReadOnlyReactiveProperty<bool> IsConnectedReactiveProperty { get; }
            public ReactiveCommand RollbackStateReactiveCommand { get; }
            public Action OnMatchEndAction { get; }
            public bool IsMultiPlayerGame { get; }
            public WindowSystem.IWindowsManager NewWindowsManager { get; }

            public Context(MatchView matchView, 
                MatchInitDataParameters matchInitDataParameters, 
                FieldConfig fieldConfig,
                MatchCommands matchCommandsEnemy, MatchCommands matchCommandsOur, MatchCommonCommands matchCommandsCommon,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty,
                ReactiveCommand quitMatchReactiveCommand,
                ReactiveCommand<int> syncFrameCounterReactiveCommand,
                IReadOnlyReactiveProperty<ProcessRoles> ourGameRoleReactiveProperty,
                IReadOnlyReactiveProperty<NetworkRoles> ourNetworkRoleReactiveProperty,
                IReadOnlyReactiveProperty<bool> isConnectedReactiveProperty,
                ReactiveCommand rollbackStateReactiveCommand,
                Action onMatchEndAction,
                bool isMultiPlayerGame,
                WindowSystem.IWindowsManager newWindowsManager)
            {
                MatchView = matchView;
                MatchInitDataParameters = matchInitDataParameters;
                FieldConfig = fieldConfig;
                MatchCommandsEnemy = matchCommandsEnemy;
                MatchCommandsOur = matchCommandsOur;
                MatchCommandsCommon = matchCommandsCommon;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
                QuitMatchReactiveCommand = quitMatchReactiveCommand;
                SyncFrameCounterReactiveCommand = syncFrameCounterReactiveCommand;
                OurGameRoleReactiveProperty = ourGameRoleReactiveProperty;
                OurNetworkRoleReactiveProperty = ourNetworkRoleReactiveProperty;
                IsConnectedReactiveProperty = isConnectedReactiveProperty;
                RollbackStateReactiveCommand = rollbackStateReactiveCommand;
                OnMatchEndAction = onMatchEndAction;
                IsMultiPlayerGame = isMultiPlayerGame;
                NewWindowsManager = newWindowsManager;
            }
        }

        private readonly Context _context;
        private readonly ConfigsRetriever _configsRetriever;
        private readonly WindowsManager _windowsManager;
        // it's important to call updates of fields in right order,
        // so here we use player1/player2 stuff instead of our/enemy
        private readonly FieldController _player1FieldController;
        private readonly FieldController _player2FieldController;
        private readonly FieldFactory _fieldFactory;
        private readonly WaveMobSpawnerCoordinator _waveMobSpawnerCoordinator;
        private readonly MatchRulesController _rulesController;
        private readonly InputController _inputController;
        private readonly MatchStateSaver _stateSaver;

        private readonly ReactiveCommand<PlayerState> _enemyStateSyncedReactiveCommand;
        private readonly ReactiveCommand<PlayerState> _ourStateSyncedReactiveCommand;

        public MatchController(Context context)
        {
            _context = context;
            
            _context.MatchView.CanvasFitter.Process();
            
            _enemyStateSyncedReactiveCommand = AddDisposable(new ReactiveCommand<PlayerState>());
            _ourStateSyncedReactiveCommand = AddDisposable(new ReactiveCommand<PlayerState>());
            
            ReactiveCommand<Hex2d> clickReactiveCommand = AddDisposable(new ReactiveCommand<Hex2d>());
            ReactiveCommand<float> matchStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<float> waveStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand waveEndedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand<float> artifactChoosingStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<int> waveNumberChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<MobConfig> spawnEnemyMobReactiveCommand = AddDisposable(new ReactiveCommand<MobConfig>());
            ReactiveCommand<MobConfig> spawnOurMobReactiveCommand = AddDisposable(new ReactiveCommand<MobConfig>());
            ReactiveProperty<bool> hasMobsOnEnemyField = AddDisposable(new ReactiveProperty<bool>(false));
            ReactiveProperty<bool> hasMobsOnOurField = AddDisposable(new ReactiveProperty<bool>(false));
            ReactiveCommand<HealthInfo> enemyCastleHealthChangedReactiveCommand = AddDisposable(new ReactiveCommand<HealthInfo>());
            ReactiveCommand<HealthInfo> ourCastleHealthChangedReactiveCommand = AddDisposable(new ReactiveCommand<HealthInfo>());
            ReactiveCommand enemyCastleDestroyedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand ourCastleDestroyedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand<int> enemyGoldenCoinsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> ourGoldenCoinsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> enemyGoldCoinsIncomeChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> ourGoldCoinsIncomeChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> enemyCrystalsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> ourCrystalsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());

            ConfigsRetriever.Context configsRetrieverContext = new ConfigsRetriever.Context(_context.FieldConfig);
            _configsRetriever = AddDisposable(new ConfigsRetriever(configsRetrieverContext));

            WaveParams[] waves =_context.MatchInitDataParameters.Waves;
            
            // windows
           WindowsManager.Context windowsControllerContext = new WindowsManager.Context(
               _context.MatchView.MatchUiViews, _context.MatchView.MainCamera, _context.MatchView.Canvas,
               _configsRetriever,
               _context.MatchInitDataParameters.HandParams,
               waves,
               
               _context.IsConnectedReactiveProperty,
               enemyCastleHealthChangedReactiveCommand,
               ourCastleHealthChangedReactiveCommand,
               matchStartedReactiveCommand,
               waveStartedReactiveCommand,
               betweenWavesPlanningStartedReactiveCommand,
               artifactChoosingStartedReactiveCommand,
               waveNumberChangedReactiveCommand,
               ourGoldenCoinsCountChangedReactiveCommand,
               ourGoldCoinsIncomeChangedReactiveCommand,
               ourCrystalsCountChangedReactiveCommand,
               _context.QuitMatchReactiveCommand,
               _context.NewWindowsManager);
           _windowsManager = AddDisposable(new WindowsManager(windowsControllerContext));

           // fields
           var hexFabric = new HexFabric(_context.FieldConfig.HexagonPrefabConfig);

           //TODO: click handle separate with field controller
           FieldController.Context enemyFieldContext = new FieldController.Context(
                _context.MatchView.EnemyFieldRoot,
                hexFabric,
                _context.MatchInitDataParameters, _context.FieldConfig,
                _configsRetriever,
                false,
                
                _context.MatchCommandsEnemy, _context.CurrentEngineFrameReactiveProperty, 
                clickReactiveCommand, _enemyStateSyncedReactiveCommand,
                spawnEnemyMobReactiveCommand,
                hasMobsOnEnemyField,
                waveNumberChangedReactiveCommand,
                waveEndedReactiveCommand,
                enemyCastleHealthChangedReactiveCommand,
                enemyCastleDestroyedReactiveCommand,
                enemyGoldenCoinsCountChangedReactiveCommand,
                enemyGoldCoinsIncomeChangedReactiveCommand,
                enemyCrystalsCountChangedReactiveCommand);

            FieldController.Context ourFieldContext = new FieldController.Context(
                _context.MatchView.OurFieldRoot,
                hexFabric,
                _context.MatchInitDataParameters, _context.FieldConfig,
                _configsRetriever,
                true,
                
                _context.MatchCommandsOur, _context.CurrentEngineFrameReactiveProperty, clickReactiveCommand, _ourStateSyncedReactiveCommand,
                spawnOurMobReactiveCommand,
                hasMobsOnOurField,
                waveNumberChangedReactiveCommand,
                waveEndedReactiveCommand,
                ourCastleHealthChangedReactiveCommand,
                ourCastleDestroyedReactiveCommand,
                ourGoldenCoinsCountChangedReactiveCommand,
                ourGoldCoinsIncomeChangedReactiveCommand,
                ourCrystalsCountChangedReactiveCommand);

            ReactiveCommand<MobConfig> spawnPlayer1MobReactiveCommand, spawnPlayer2MobReactiveCommand;
            MatchCommands player1MatchCommands, player2MatchCommands;

            if (_context.OurGameRoleReactiveProperty.Value == ProcessRoles.Player1)
            {
                _player1FieldController = AddDisposable(new FieldController(ourFieldContext));
                spawnPlayer1MobReactiveCommand = spawnOurMobReactiveCommand;
                player1MatchCommands = _context.MatchCommandsOur;
                _player2FieldController = AddDisposable(new FieldController(enemyFieldContext));
                spawnPlayer2MobReactiveCommand = spawnEnemyMobReactiveCommand;
                player2MatchCommands = _context.MatchCommandsEnemy;
            }
            else
            {
                _player1FieldController = AddDisposable(new FieldController(enemyFieldContext));
                spawnPlayer1MobReactiveCommand = spawnEnemyMobReactiveCommand;
                player1MatchCommands = _context.MatchCommandsEnemy;
                _player2FieldController = AddDisposable(new FieldController(ourFieldContext));
                spawnPlayer2MobReactiveCommand = spawnOurMobReactiveCommand;
                player2MatchCommands = _context.MatchCommandsOur;
            }
            
            // wave mob spawner
            WaveMobSpawnerCoordinator.Context waveMobSpawnerContext = new WaveMobSpawnerCoordinator.Context(
                _configsRetriever,
                _context.FieldConfig,
                _context.MatchCommandsCommon.IncomingGeneral,
                player1MatchCommands.Incoming,
                player2MatchCommands.Incoming,
                _context.MatchCommandsCommon.Server,
                waves,
                _context.IsMultiPlayerGame,
                _context.OurNetworkRoleReactiveProperty,

                matchStartedReactiveCommand,
                waveStartedReactiveCommand,
                waveEndedReactiveCommand,
                artifactChoosingStartedReactiveCommand,
                betweenWavesPlanningStartedReactiveCommand,
                waveNumberChangedReactiveCommand,
                spawnPlayer1MobReactiveCommand,
                spawnPlayer2MobReactiveCommand,
                hasMobsOnEnemyField,
                hasMobsOnOurField);
            _waveMobSpawnerCoordinator = new WaveMobSpawnerCoordinator(waveMobSpawnerContext);

            // rules
            //MatchRulesController.Context rulesControllerContext = new MatchRulesController.Context(
            //    _windowsManager.WinLoseWindowController,
            //    enemyCastleDestroyedReactiveCommand,
            //    ourCastleDestroyedReactiveCommand);
            //_rulesController = AddDisposable(new MatchRulesController(rulesControllerContext));

            // input
            HexInteractService hexInteractService = new HexInteractService(_context.MatchView.MainCamera);
            
            InputController.Context inputControllerContext = new InputController.Context(
                hexInteractService, clickReactiveCommand);
            _inputController = AddDisposable(new InputController(inputControllerContext));
            
            // state saver
            MatchStateSaver.Context stateSaverContext = new MatchStateSaver.Context(
                _player1FieldController, _player2FieldController, _waveMobSpawnerCoordinator,
                waveStartedReactiveCommand);
            _stateSaver = AddDisposable(new MatchStateSaver(stateSaverContext));

            // subscribe photon actions
            enemyCastleDestroyedReactiveCommand.Subscribe((u) => _context.OnMatchEndAction());
            ourCastleDestroyedReactiveCommand.Subscribe((u) => _context.OnMatchEndAction());

            _context.MatchCommandsCommon.IncomingGeneral.RequestSyncState.Subscribe(SendState);
            _context.MatchCommandsCommon.IncomingGeneral.ApplySyncState.Subscribe(SyncState);

            _context.RollbackStateReactiveCommand.Subscribe(RollbackState);
        }        

        private void SendState()
        {
            _context.MatchCommandsCommon.Server.SendState.Fire(_stateSaver.GetCurrentMatchState());
        }
        
        private void SyncState(MatchState matchState, int timeStamp)
        {
            _context.SyncFrameCounterReactiveCommand.Execute(timeStamp);
            _player1FieldController.Reset();
            _player2FieldController.Reset();

            if (_context.OurGameRoleReactiveProperty.Value == ProcessRoles.Player1)
            {
                _enemyStateSyncedReactiveCommand.Execute(matchState.Player2State);
                _ourStateSyncedReactiveCommand.Execute(matchState.Player1State);
            }
            else
            {
                _enemyStateSyncedReactiveCommand.Execute(matchState.Player1State);
                _ourStateSyncedReactiveCommand.Execute(matchState.Player2State);
            }

            _waveMobSpawnerCoordinator.LoadState(matchState.WavesState);

            if (_waveMobSpawnerCoordinator.CurrentWaveNumber != matchState.WavesState.CurrentWaveNumber)
            {
                // TODO: some logic to sync waves
                Randomizer.InitState(matchState.RandomSeed);
            }
                
            // rewind Randomizer to target counter value
            while (Randomizer.RandomCallsCountReactiveProperty.Value < matchState.RandomCounter)
                Randomizer.GetRandomInRange(0, 1);
        }

        private void RollbackState(Unit unit)
        {
            SyncState(_stateSaver.GetLastSavedMatchState(), _context.CurrentEngineFrameReactiveProperty.Value);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            //if (!_rulesController.IsMatchRunning)
            //    return;
            
            _inputController.OuterLogicUpdate(frameLength);
            _waveMobSpawnerCoordinator.OuterLogicUpdate(frameLength);
            _windowsManager.OuterLogicUpdate(frameLength);
            
            // the order is important due to calls to Random inside
            _player1FieldController.OuterLogicUpdate(frameLength);
            
            if (_context.IsMultiPlayerGame)
                _player2FieldController.OuterLogicUpdate(frameLength);
        }

        public void OuterViewUpdate(float frameLength)
        {
            _player1FieldController.OuterViewUpdate(frameLength);
            
            if (_context.IsMultiPlayerGame)
                _player2FieldController.OuterViewUpdate(frameLength);
        }
    }
}
