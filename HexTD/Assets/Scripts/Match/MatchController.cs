using BuffLogic;
using Extensions;
using HexSystem;
using MapEditor;
using Match.Commands;
using Match.Field;
using Match.Field.Castle;
using Match.Field.Hand;
using Match.Field.Mob;
using Match.Field.State;
using Match.Field.Tower;
using Match.Field.VFX;
using Match.State;
using Match.State.CheckSum;
using Match.Wave;
using Services;
using Tools;
using Tools.Interfaces;
using UI.ScreenSpaceOverlaySystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace Match
{
    public class MatchController : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public MatchInitDataParameters MatchInitDataParameters { get; }
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
            public bool IsMultiPlayerGame { get; }
            public WindowSystem.IWindowsManager WindowsManager { get; }

            public Context(
                MatchInitDataParameters matchInitDataParameters,
                MatchCommands matchCommandsEnemy, MatchCommands matchCommandsOur, MatchCommonCommands matchCommandsCommon,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty,
                ReactiveCommand quitMatchReactiveCommand,
                ReactiveCommand<int> syncFrameCounterReactiveCommand,
                IReadOnlyReactiveProperty<ProcessRoles> ourGameRoleReactiveProperty,
                IReadOnlyReactiveProperty<NetworkRoles> ourNetworkRoleReactiveProperty,
                IReadOnlyReactiveProperty<bool> isConnectedReactiveProperty,
                ReactiveCommand rollbackStateReactiveCommand,
                bool isMultiPlayerGame,
                WindowSystem.IWindowsManager windowsManager)
            {
                MatchInitDataParameters = matchInitDataParameters;
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
                IsMultiPlayerGame = isMultiPlayerGame;
                WindowsManager = windowsManager;
            }
        }

        private readonly Context _context;
        private readonly ConfigsRetriever _configsRetriever;
        private readonly PlayerHandController _ourPlayerHandController;
        private readonly PlayerHandController _enemyPlayerHandController;
        private readonly WindowsManager _windowsManager;
        // it's important to call updates of fields in right order,
        // so here we use player1/player2 stuff instead of our/enemy
        private readonly BuffManager _buffManager;
        private readonly VfxManager _vfxManager;
        private readonly FieldController _player1FieldController;
        private readonly FieldController _player2FieldController;
        private readonly FieldFactory _fieldFactory;
        private readonly WaveMobSpawnerCoordinator _waveMobSpawnerCoordinator;
        private readonly MatchRulesController _rulesController;
        private readonly InputController _inputController;
        private readonly FieldClicksHandler _clicksHandler;
        private readonly FieldClicksDistributor _ourClicksDistributor;
        private readonly FieldClicksDistributor _enemyClicksDistributor;
        private readonly TowerPlacer _ourTowerPlacer;
        private readonly MatchStateCheckSumComputerController _checkSumComputerController;
        private readonly MatchStateSaver _stateSaver;
        
        private readonly MatchView _matchView;
        private readonly FieldConfig _fieldConfig;
            
        private readonly FieldController.Factory _fieldControllerFactory;
        private readonly ScreenSpaceOverlayController _screenSpaceOverlayController;

        private readonly ReactiveCommand<PlayerState> _enemyStateSyncedReactiveCommand;
        private readonly ReactiveCommand<PlayerState> _ourStateSyncedReactiveCommand;

        public MatchController(Context context, MatchView matchView, FieldController.Factory fieldControllerFactory,
            ScreenSpaceOverlayController screenSpaceOverlayController)
        {
            _context = context;

            _matchView = matchView;
            _fieldConfig = matchView.FieldConfig;
                
            _fieldControllerFactory = fieldControllerFactory;
            _screenSpaceOverlayController = screenSpaceOverlayController;
            
            _matchView.CanvasFitter.Process();
            
            _enemyStateSyncedReactiveCommand = AddDisposable(new ReactiveCommand<PlayerState>());
            _ourStateSyncedReactiveCommand = AddDisposable(new ReactiveCommand<PlayerState>());
            
            ReactiveCommand<Hex2d> clickReactiveCommand = AddDisposable(new ReactiveCommand<Hex2d>());
            ReactiveCommand<float> matchStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<float> waveStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand waveEndedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand<float> artifactChoosingStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand = AddDisposable(new ReactiveCommand<float>());
            ReactiveCommand<int> waveNumberChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<MobSpawnParameters> spawnEnemyMobReactiveCommand = AddDisposable(new ReactiveCommand<MobSpawnParameters>());
            ReactiveCommand<MobSpawnParameters> spawnOurMobReactiveCommand = AddDisposable(new ReactiveCommand<MobSpawnParameters>());
            ReactiveProperty<bool> hasMobsOnEnemyField = AddDisposable(new ReactiveProperty<bool>(false));
            ReactiveProperty<bool> hasMobsOnOurField = AddDisposable(new ReactiveProperty<bool>(false));
            ReactiveCommand<HealthInfo> enemyCastleHealthChangedReactiveCommand = AddDisposable(new ReactiveCommand<HealthInfo>());
            ReactiveCommand<HealthInfo> ourCastleHealthChangedReactiveCommand = AddDisposable(new ReactiveCommand<HealthInfo>());
            ReactiveCommand enemyCastleDestroyedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand ourCastleDestroyedReactiveCommand = AddDisposable(new ReactiveCommand());
            ReactiveCommand<int> enemyGoldenCoinsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> ourGoldenCoinsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> enemyCrystalsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<int> ourCrystalsCountChangedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<bool> dragCardChangeStatusCommand = AddDisposable(new ReactiveCommand<bool>());
            ReactiveCommand<Hex2d> placeForOurTowerSelectedCommand = AddDisposable(new ReactiveCommand<Hex2d>());

            ConfigsRetriever.Context configsRetrieverContext = new ConfigsRetriever.Context(_fieldConfig);
            _configsRetriever = AddDisposable(new ConfigsRetriever(configsRetrieverContext));

            MatchConfig matchConfig = _configsRetriever.GetLevelById(_context.MatchInitDataParameters.LevelId);
            MapLoader loader = new MapLoader();
            LevelMapModel mapModel = loader.LoadMapFromAsset(matchConfig.LevelMap);

            _ourPlayerHandController = new PlayerHandController(
                _context.MatchInitDataParameters.PlayerHandParams.Towers,
                matchConfig.EnergyStartCount,
                matchConfig.EnergyRestoreDelay,
                MatchConfig.EnergyRestoreValue,
                MatchConfig.EnergyMaxCount);
            
            _enemyPlayerHandController = new PlayerHandController(
                _context.MatchInitDataParameters.PlayerHandParams.Towers,
                matchConfig.EnergyStartCount,
                matchConfig.EnergyRestoreDelay,
                MatchConfig.EnergyRestoreValue,
                MatchConfig.EnergyMaxCount);
            
            // windows
            WindowsManager.Context windowsControllerContext = new WindowsManager.Context(
                _matchView.MatchUiViews, _matchView.Canvas,
               _configsRetriever,
                _context.IsMultiPlayerGame,
               _ourPlayerHandController,
               matchConfig.Waves,
               
               _context.IsConnectedReactiveProperty,
               enemyCastleHealthChangedReactiveCommand,
               ourCastleHealthChangedReactiveCommand,
               matchStartedReactiveCommand,
               waveStartedReactiveCommand,
               betweenWavesPlanningStartedReactiveCommand,
               artifactChoosingStartedReactiveCommand,
               waveNumberChangedReactiveCommand,
               ourGoldenCoinsCountChangedReactiveCommand,
               ourCrystalsCountChangedReactiveCommand,
               dragCardChangeStatusCommand,
               _context.QuitMatchReactiveCommand,
               _context.WindowsManager);
           _windowsManager = AddDisposable(new WindowsManager(windowsControllerContext));

           _buffManager = new BuffManager();
           _vfxManager = new VfxManager();

           // fields
           var hexFabric = new HexObjectFabric(_configsRetriever);
           var propsFabric = new PropsObjectFabric(_configsRetriever);

           //TODO: click handle separate with field controller
           FieldController.Context enemyFieldContext = new FieldController.Context(
               _matchView.EnemyFieldRoot,
                hexFabric,
                propsFabric,
                _fieldConfig,
                matchConfig,
                mapModel,
                _configsRetriever,
                _buffManager,
                _vfxManager,
                _context.IsMultiPlayerGame,
               false,
                
                _enemyStateSyncedReactiveCommand,
                spawnEnemyMobReactiveCommand,
                hasMobsOnEnemyField,
                enemyCastleHealthChangedReactiveCommand,
                enemyCastleDestroyedReactiveCommand,
                enemyGoldenCoinsCountChangedReactiveCommand,
                enemyCrystalsCountChangedReactiveCommand);
               
            FieldController.Context ourFieldContext = new FieldController.Context(
                _matchView.OurFieldRoot,
                hexFabric,
                propsFabric,
                _fieldConfig,
                matchConfig,
                mapModel,
                _configsRetriever,
                _buffManager,
                _vfxManager,
                true,
                true,
                
                _ourStateSyncedReactiveCommand,
                spawnOurMobReactiveCommand,
                hasMobsOnOurField,
                ourCastleHealthChangedReactiveCommand,
                ourCastleDestroyedReactiveCommand,
                ourGoldenCoinsCountChangedReactiveCommand,
                ourCrystalsCountChangedReactiveCommand);

            ReactiveCommand<MobSpawnParameters> spawnPlayer1MobReactiveCommand, spawnPlayer2MobReactiveCommand;
            MatchCommands player1MatchCommands, player2MatchCommands;

            FieldController ourField, enemyField;

            if (_context.OurGameRoleReactiveProperty.Value == ProcessRoles.Player1)
            {
                _player1FieldController = AddDisposable(_fieldControllerFactory.Create(ourFieldContext));
                spawnPlayer1MobReactiveCommand = spawnOurMobReactiveCommand;
                player1MatchCommands = _context.MatchCommandsOur;
                _player2FieldController = AddDisposable(_fieldControllerFactory.Create(enemyFieldContext));
                spawnPlayer2MobReactiveCommand = spawnEnemyMobReactiveCommand;
                player2MatchCommands = _context.MatchCommandsEnemy;
                ourField = _player1FieldController;
                enemyField = _player2FieldController;
            }
            else
            {
                _player1FieldController = AddDisposable(_fieldControllerFactory.Create(enemyFieldContext));
                spawnPlayer1MobReactiveCommand = spawnEnemyMobReactiveCommand;
                player1MatchCommands = _context.MatchCommandsEnemy;
                _player2FieldController = AddDisposable(_fieldControllerFactory.Create(ourFieldContext));
                spawnPlayer2MobReactiveCommand = spawnOurMobReactiveCommand;
                player2MatchCommands = _context.MatchCommandsOur;
                ourField = _player2FieldController;
                enemyField = _player1FieldController;
            }
            
            // setup our and enemy cameras
            {
                var ourFieldBounds = ourField.GetFieldBounds();
                DebugDrawingTools.DrawBounds(ourFieldBounds, Color.white, 5.0f);

                if (_matchView.OurFieldCamera.TryGetFocusTransforms(ourFieldBounds, out var pos,
                        out var rot))
                    _matchView.OurFieldCamera.transform.SetPositionAndRotation(
                        pos - _matchView.OurFieldCamera.transform.up * 5.0f,
                        rot);
            }
            {
                var enemyFieldBounds = enemyField.GetFieldBounds();
                DebugDrawingTools.DrawBounds(enemyFieldBounds, Color.white, 5.0f);

                if (_matchView.EnemyFieldCamera.TryGetFocusTransforms(enemyFieldBounds, out var pos,
                        out var rot))
                    _matchView.EnemyFieldCamera.transform.SetPositionAndRotation(
                        pos,
                        rot);
            }

            ourField.InitPlayerHand(_ourPlayerHandController);
            enemyField.InitPlayerHand(_enemyPlayerHandController);
            
            // wave mob spawner
            WaveMobSpawnerCoordinator.Context waveMobSpawnerContext = new WaveMobSpawnerCoordinator.Context(
                _configsRetriever,
                _fieldConfig,
                _context.MatchCommandsCommon.IncomingGeneral,
                player1MatchCommands.Incoming,
                player2MatchCommands.Incoming,
                _context.MatchCommandsCommon.Server,
                matchConfig.Waves,
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

            // input
            HexInteractService hexInteractService = new HexInteractService(_matchView.OurFieldCamera);
            
            InputController.Context inputControllerContext = new InputController.Context(
                hexInteractService, clickReactiveCommand);
            _inputController = AddDisposable(new InputController(inputControllerContext));
            
            FieldClicksHandler.Context clicksHandlerContext = new FieldClicksHandler.Context(
                clickReactiveCommand);
            _clicksHandler = AddDisposable(new FieldClicksHandler(clicksHandlerContext));

            TowerPlacer.Context ourTowerPlacerContext =
                new TowerPlacer.Context(
                    ourField.FieldConstructionProcessController,
                    _ourPlayerHandController,
                    _configsRetriever,
                    ourField.HexagonalFieldModel,
                    ourField.PathContainer,
                    
                    dragCardChangeStatusCommand,
                    placeForOurTowerSelectedCommand);
            _ourTowerPlacer = AddDisposable(new TowerPlacer(ourTowerPlacerContext));
            
            FieldClicksDistributor.Context ourClicksDistributorContext =
                new FieldClicksDistributor.Context(
                    ourField.FieldModel, _clicksHandler, _configsRetriever,
                    ourField.FieldConstructionProcessController,
                    _ourPlayerHandController, _ourTowerPlacer, _context.MatchCommandsOur,
                    _windowsManager.TowerManipulationWindowController,
                    placeForOurTowerSelectedCommand
                );
            _ourClicksDistributor = AddDisposable(new FieldClicksDistributor(ourClicksDistributorContext));

            FieldClicksDistributor.Context enemyClicksDistributorContext =
                new FieldClicksDistributor.Context(
                    enemyField.FieldModel, null, _configsRetriever,
                    enemyField.FieldConstructionProcessController,
                    _enemyPlayerHandController, null, _context.MatchCommandsEnemy,
                    _windowsManager.TowerManipulationWindowController,
                    placeForOurTowerSelectedCommand
                );
            _enemyClicksDistributor = AddDisposable(new FieldClicksDistributor(enemyClicksDistributorContext));

            // rules
            MatchRulesController.Context rulesControllerContext = new MatchRulesController.Context(
                _windowsManager.WinLoseWindowController,
                enemyCastleDestroyedReactiveCommand,
                ourCastleDestroyedReactiveCommand);
            _rulesController = AddDisposable(new MatchRulesController(rulesControllerContext));
            
            // state saver
            _checkSumComputerController = AddDisposable(new MatchStateCheckSumComputerController());
            
            MatchStateSaver.Context stateSaverContext = new MatchStateSaver.Context(
                _player1FieldController, _player2FieldController,
                _checkSumComputerController, _waveMobSpawnerCoordinator,
                waveStartedReactiveCommand, matchStateCheckSumComputedReactiveCommand, _context.CurrentEngineFrameReactiveProperty);
            _stateSaver = AddDisposable(new MatchStateSaver(stateSaverContext));

            _context.MatchCommandsCommon.IncomingGeneral.RequestSyncState.Subscribe(SendState);
            _context.MatchCommandsCommon.IncomingGeneral.ApplySyncState.Subscribe(SyncState);

            _context.RollbackStateReactiveCommand.Subscribe(RollbackState);
        }

        private void SendState()
        {
            _context.MatchCommandsCommon.Server.SendState.Fire(_stateSaver.GetCurrentMatchState());
        }
        
        private void SyncState(MatchState matchState, int frameCounter)
        {
            _context.SyncFrameCounterReactiveCommand.Execute(frameCounter);

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
            if (!_rulesController.IsMatchRunning)
                return;
            
            _ourClicksDistributor.OuterLogicUpdate(frameLength);

            _waveMobSpawnerCoordinator.OuterLogicUpdate(frameLength);
            _buffManager.OuterLogicUpdate(frameLength);

            _ourPlayerHandController.OuterLogicUpdate(frameLength);
            _windowsManager.OuterLogicUpdate(frameLength);
            _vfxManager.OuterLogicUpdate(frameLength);
            
            // the order is important due to calls to Random inside
            _player1FieldController.OuterLogicUpdate(frameLength);
            
            if (_context.IsMultiPlayerGame)
                _player2FieldController.OuterLogicUpdate(frameLength);

            _rulesController.OuterLogicUpdate(frameLength);
        }

        public void OuterViewUpdate(float frameLength)
        {
            _player1FieldController.OuterViewUpdate(frameLength);
            
            if (_context.IsMultiPlayerGame)
                _player2FieldController.OuterViewUpdate(frameLength);
            
            _screenSpaceOverlayController.OuterViewUpdate(frameLength);
        }
        
        public class Factory : PlaceholderFactory<Context, MatchController>
        {
        }
    }
}
