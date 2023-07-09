using ExitGames.Client.Photon;
using Match.Commands.Implementations;
using Match.EventBus;
using Services;
using Tools;
using UniRx;

namespace Match.Commands
{
    public class CommandExecutorsAggregator : BaseDisposable
    {
        public struct Context
        {
            public TestMatchEngine MatchEngine { get; }
            public IEventBus EventBus { get; }
            public NetworkMatchStatus NetworkMatchStatus { get; }
            public IReadOnlyReactiveProperty<int> PingDamperFramesDeltaReactiveProperty { get; }

            public Context(TestMatchEngine matchEngine,
                IEventBus eventBus,
                NetworkMatchStatus networkMatchStatus,
                IReadOnlyReactiveProperty<int> pingDamperFramesDeltaReactiveProperty)
            {
                MatchEngine = matchEngine;
                EventBus = eventBus;
                NetworkMatchStatus = networkMatchStatus;
                PingDamperFramesDeltaReactiveProperty = pingDamperFramesDeltaReactiveProperty;
            }
        }

        private readonly Context _context;

        private SyncStateRequestCommandExecutor _syncStateRequestCommandExecutor;
        private SyncStateApplyCommandExecutor _syncStateApplyCommandExecutor;
        private BuildTowerCommandExecutor _buildTowerCommandExecutor;
        private UpgradeTowerCommandExecutor _upgradeTowerCommandExecutor;
        private SellTowerCommandExecutor _sellTowerCommandExecutor;
        private StartWaveSpawnCommandExecutor _startWaveSpawnCommandExecutor;
        private BroadcastStateCheckSumCommandExecutor _broadcastStateCheckSumCommandExecutor;

        public CommandExecutorsAggregator(Context context)
        {
            _context = context;
            
            CreateCommandExecutors();
        }
        
        private void CreateCommandExecutors()
        {
            AbstractCommandExecutor.Context commandContext = new AbstractCommandExecutor.Context(_context.MatchEngine, _context.EventBus,
                _context.MatchEngine.CurrentEngineFrameReactiveProperty, _context.PingDamperFramesDeltaReactiveProperty, _context.NetworkMatchStatus.CurrentProcessNetworkRoleReactiveProperty);
            _syncStateRequestCommandExecutor = new SyncStateRequestCommandExecutor(commandContext);
            _syncStateApplyCommandExecutor = new SyncStateApplyCommandExecutor(commandContext);
            _buildTowerCommandExecutor = new BuildTowerCommandExecutor(commandContext);
            _upgradeTowerCommandExecutor = new UpgradeTowerCommandExecutor(commandContext);
            _sellTowerCommandExecutor = new SellTowerCommandExecutor(commandContext);
            _startWaveSpawnCommandExecutor = new StartWaveSpawnCommandExecutor(commandContext);
            _broadcastStateCheckSumCommandExecutor = new BroadcastStateCheckSumCommandExecutor(commandContext);
        }
        
        public async void ProcessEvent(byte eventCode, Hashtable parametersTable, int senderId)
        {
            switch (eventCode)
            {
                // sync state
                case PhotonEventsConstants.SyncState.RequestEventId:
                    await _syncStateRequestCommandExecutor.Request(parametersTable);
                    break;
                case PhotonEventsConstants.SyncState.ApplyEventId:
                    await _syncStateApplyCommandExecutor.Apply(parametersTable);
                    break;
                
                // build tower
                case PhotonEventsConstants.BuildTower.RequestEventId:
                    await _buildTowerCommandExecutor.Request(parametersTable);
                    break;
                case PhotonEventsConstants.BuildTower.ApplyEventId:
                    await _buildTowerCommandExecutor.Apply(parametersTable);
                    break;
                
                // upgrade tower
                case PhotonEventsConstants.UpgradeTower.RequestEventId:
                    await _upgradeTowerCommandExecutor.Request(parametersTable);
                    break;
                case PhotonEventsConstants.UpgradeTower.ApplyEventId:
                    await _upgradeTowerCommandExecutor.Apply(parametersTable);
                    break;
                
                // sell tower
                case PhotonEventsConstants.SellTower.RequestEventId:
                    await _sellTowerCommandExecutor.Request(parametersTable);
                    break;
                case PhotonEventsConstants.SellTower.ApplyEventId:
                    await _sellTowerCommandExecutor.Apply(parametersTable);
                    break;

                // start wave spawn
                case PhotonEventsConstants.StartWaveSpawn.ApplyEventId:
                    await _startWaveSpawnCommandExecutor.Apply(parametersTable);
                    break;
                
                // broadcast state checksum
                case PhotonEventsConstants.BroadcastStateCheckSum.ApplyEventId:
                    await _broadcastStateCheckSumCommandExecutor.Apply(parametersTable);
                    break;
            }
        }
    }
}