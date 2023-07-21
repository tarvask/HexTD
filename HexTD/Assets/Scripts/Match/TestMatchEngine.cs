using System;
using Match.Commands;
using Match.EventBus;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

namespace Match
{
    public class TestMatchEngine : BaseMonoBehaviour
    {
        public const byte LogicFramesPerSecond = 60;
        public const float FrameLength = 1f / LogicFramesPerSecond;
        public const byte TowersInHandCount = 8;
        
        [SerializeField] private MatchView matchPrefab;

        private MatchController _matchController;
        private OutgoingCommandsProcessor _outgoingCommandsProcessor;
        private IncomingCommandsProcessor _incomingCommandsProcessor;
        private ServerCommandsProcessor _serverCommandsProcessor;
        private ReactiveCommand _rollbackStateReactiveCommand;
        private Action _onRequestMatchStateAction;
        private Action _onQuitGameAction;
        private Action _onMatchEndAction;
        
        private float _currentEngineFrameTimestamp;
        private bool _isInited;
        private bool _isConnected;

        public bool IsInited => _isInited;

        public IncomingCommandsProcessor IncomingCommandsProcessor => _incomingCommandsProcessor;
        public ReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; private set; }

        private MatchController.Factory _matchControllerFactory;
        private WindowSystem.IWindowsManager _newWindowsManager;
        
        [Inject]
        public void Constructor(
            MatchController.Factory matchControllerFactory,
            WindowSystem.IWindowsManager newWindowsManager)
        {
            _matchControllerFactory = matchControllerFactory;
            
            _newWindowsManager = newWindowsManager;
        }

        private void Awake()
        {
            Application.targetFrameRate = LogicFramesPerSecond;
        }

        public void Init(MatchInitDataParameters matchInitDataParameters, IEventBus eventBus,
            IReadOnlyReactiveProperty<ProcessRoles> currentProcessGameRoleReactiveProperty,
            IReadOnlyReactiveProperty<NetworkRoles> currentProcessNetworkRoleReactiveProperty,
            IReadOnlyReactiveProperty<bool> isConnectedReactiveProperty,
            Action onRequestStateAction,
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

            ReactiveCommand requestMatchStateReactiveCommand = new ReactiveCommand();
            ReactiveCommand quitMatchReactiveCommand = new ReactiveCommand();
            ReactiveCommand<int> syncFrameCounterCommand = new ReactiveCommand<int>();
            _rollbackStateReactiveCommand = new ReactiveCommand();
            
            MatchController.Context matchControllerContext = new MatchController.Context( matchInitDataParameters,
                matchCommandsEnemy, matchCommandsOur, matchCommonCommands,
                CurrentEngineFrameReactiveProperty, requestMatchStateReactiveCommand, quitMatchReactiveCommand, syncFrameCounterCommand,
                currentProcessGameRoleReactiveProperty, currentProcessNetworkRoleReactiveProperty, isConnectedReactiveProperty,
                _rollbackStateReactiveCommand,
                isMultiPlayerGame,
                _newWindowsManager);
            _matchController = _matchControllerFactory.Create(matchControllerContext);

            OutgoingCommandsProcessor.Context outgoingCommandsProcessorContext = new OutgoingCommandsProcessor.Context(
                this, eventBus, currentProcessGameRoleReactiveProperty, matchOutgoingCommands);
            _outgoingCommandsProcessor = new OutgoingCommandsProcessor(outgoingCommandsProcessorContext);
            
            IncomingCommandsProcessor.Context incomingCommandsProcessorContext = new IncomingCommandsProcessor.Context(
                matchIncomingCommandsEnemy, matchIncomingCommandsOur, matchIncomingCommandsCommon, currentProcessGameRoleReactiveProperty);
            _incomingCommandsProcessor = new IncomingCommandsProcessor(incomingCommandsProcessorContext);
            
            ServerCommandsProcessor.Context serverCommandsProcessorContext = new ServerCommandsProcessor.Context(
                this, eventBus, matchCommonCommands.Server);
            _serverCommandsProcessor = new ServerCommandsProcessor(serverCommandsProcessorContext);
            
            _onRequestMatchStateAction = onRequestStateAction;
            _onQuitGameAction = onQuitGameAction;
            _onMatchEndAction = onMatchEndAction;

            requestMatchStateReactiveCommand.Subscribe((Unit unit) => RequestMatchState());
            quitMatchReactiveCommand.Subscribe((Unit unit) => QuitMatch());
            syncFrameCounterCommand.Subscribe(SyncFrameCounter);

            CurrentEngineFrameReactiveProperty.Value = 0;
            _currentEngineFrameTimestamp = Time.time;

            isConnectedReactiveProperty.Subscribe(UpdateConnectionState);
            
            _isInited = true;
            Debug.Log($"Engine inited, frame is {CurrentEngineFrameReactiveProperty.Value}");
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

            if (_currentEngineFrameTimestamp + FrameLength <= Time.time)
            {
                _currentEngineFrameTimestamp = Time.time;
                CurrentEngineFrameReactiveProperty.Value++;
            }
            
            _matchController.OuterLogicUpdate(Time.deltaTime);
        }

        private void RequestMatchState()
        {
            _onRequestMatchStateAction();
        }

        private void QuitMatch()
        {
            _matchController.Dispose();
            _onMatchEndAction();
            _onQuitGameAction();
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