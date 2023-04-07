using System;
using Configs;
using Match.Commands;
using Match.EventBus;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match
{
    public class TestMatchEngine : BaseMonoBehaviour
    {
        public const float FrameLength = 0.5f;
        
        [SerializeField] private MatchView matchPrefab;
        [SerializeField] private string[] matchScenesNames;

        private MatchController _matchController;
        private OutgoingCommandsProcessor _outgoingCommandsProcessor;
        private IncomingCommandsProcessor _incomingCommandsProcessor;
        private ServerCommandsProcessor _serverCommandsProcessor;
        private ReactiveCommand _rollbackStateReactiveCommand;
        private Action _onQuitGameAction;
        
        private float _currentEngineFrameTimestamp;
        private bool _isInited;
        private bool _isConnected;

        public bool IsInited => _isInited;

        public IncomingCommandsProcessor IncomingCommandsProcessor => _incomingCommandsProcessor;
        public ReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; private set; }

        public void Init(MatchInitDataParameters matchShortParameters, IEventBus eventBus,
            IReadOnlyReactiveProperty<ProcessRoles> currentProcessGameRoleReactiveProperty,
            IReadOnlyReactiveProperty<NetworkRoles> currentProcessNetworkRoleReactiveProperty,
            IReadOnlyReactiveProperty<bool> isConnectedReactiveProperty,
            Action onMatchEndAction,
            Action onQuitGameAction,
            bool isMultiPlayerGame)
        {
            CurrentEngineFrameReactiveProperty = new ReactiveProperty<int>();
            
            MatchCommands.OutgoingCommands matchOutgoingCommands = new MatchCommands.OutgoingCommands();
            MatchCommands.IncomingCommands matchIncomingCommandsEnemy = new MatchCommands.IncomingCommands();
            MatchCommands.IncomingCommands matchIncomingCommandsOur = new MatchCommands.IncomingCommands();
            MatchCommands matchCommandsEnemy = new MatchCommands(matchOutgoingCommands, matchIncomingCommandsEnemy);
            MatchCommands matchCommandsOur = new MatchCommands(matchOutgoingCommands, matchIncomingCommandsOur);
            
            MatchCommonCommands.ServerCommands matchServerCommands = new MatchCommonCommands.ServerCommands();
            MatchCommonCommands.IncomingGeneralCommands matchIncomingCommandsCommon = new MatchCommonCommands.IncomingGeneralCommands();
            MatchCommonCommands matchCommonCommands = new MatchCommonCommands(matchServerCommands, matchIncomingCommandsCommon);
            
            ReactiveCommand quitMatchReactiveCommand = new ReactiveCommand();
            ReactiveCommand<int> syncFrameCounterCommand = new ReactiveCommand<int>();
            _rollbackStateReactiveCommand = new ReactiveCommand();
            
            MatchView matchView = Instantiate(matchPrefab, Vector3.zero, Quaternion.identity, transform);
            MatchController.Context matchControllerContext = new MatchController.Context(matchView, matchShortParameters, matchView.FieldConfig,
                matchCommandsEnemy, matchCommandsOur, matchCommonCommands,
                CurrentEngineFrameReactiveProperty, quitMatchReactiveCommand, syncFrameCounterCommand,
                currentProcessGameRoleReactiveProperty, currentProcessNetworkRoleReactiveProperty, isConnectedReactiveProperty,
                _rollbackStateReactiveCommand,
                onMatchEndAction, isMultiPlayerGame);
            _matchController = new MatchController(matchControllerContext);

            OutgoingCommandsProcessor.Context outgoingCommandsProcessorContext = new OutgoingCommandsProcessor.Context(
                this, eventBus, currentProcessGameRoleReactiveProperty, matchOutgoingCommands);
            _outgoingCommandsProcessor = new OutgoingCommandsProcessor(outgoingCommandsProcessorContext);
            
            IncomingCommandsProcessor.Context incomingCommandsProcessorContext = new IncomingCommandsProcessor.Context(
                matchIncomingCommandsEnemy, matchIncomingCommandsOur, matchIncomingCommandsCommon, currentProcessGameRoleReactiveProperty);
            _incomingCommandsProcessor = new IncomingCommandsProcessor(incomingCommandsProcessorContext);
            
            ServerCommandsProcessor.Context serverCommandsProcessorContext = new ServerCommandsProcessor.Context(
                this, eventBus, matchCommonCommands.Server);
            _serverCommandsProcessor = new ServerCommandsProcessor(serverCommandsProcessorContext);

            quitMatchReactiveCommand.Subscribe((Unit unit) => QuitMatch());
            syncFrameCounterCommand.Subscribe(SyncFrameCounter);
            _onQuitGameAction = onQuitGameAction;

            CurrentEngineFrameReactiveProperty.Value = 0;
            _currentEngineFrameTimestamp = Time.time;

            isConnectedReactiveProperty.Subscribe(UpdateConnectionState);
            
            _isInited = true;
        }

        public void RollbackState()
        {
            _rollbackStateReactiveCommand.Execute();
        }

        private void Update()
        {
            if (!_isInited || !_isConnected)
                return;
            
            _matchController.OuterViewUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (!_isInited || !_isConnected)
                return;

            if (_currentEngineFrameTimestamp + FrameLength >= Time.time)
            {
                _currentEngineFrameTimestamp = Time.time;
                CurrentEngineFrameReactiveProperty.Value++;
            }
            
            _matchController.OuterLogicUpdate(Time.deltaTime);
        }

        private void QuitMatch()
        {
            _matchController.Dispose();
            _onQuitGameAction();
            
            foreach (string sceneName in matchScenesNames)
                SceneManager.UnloadSceneAsync(sceneName);
        }

        private void UpdateConnectionState(bool isConnected)
        {
            _isConnected = isConnected;
        }

        private void SyncFrameCounter(int currentFrame)
        {
            CurrentEngineFrameReactiveProperty.Value = currentFrame;
            _currentEngineFrameTimestamp = Time.time;
        }
    }
}