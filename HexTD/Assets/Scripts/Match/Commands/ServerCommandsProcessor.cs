using ExitGames.Client.Photon;
using Match.EventBus;
using Match.State;
using Match.State.CheckSum;
using Match.Wave;
using Tools;

namespace Match.Commands
{
    public class ServerCommandsProcessor : BaseDisposable
    {
        public struct Context
        {
            public TestMatchEngine TestMatchEngine { get; }
            public IEventBus EventBus { get; }
            public MatchCommonCommands.ServerCommands ServerCommands { get; }

            public Context(TestMatchEngine testMatchEngine, IEventBus eventBus,
                MatchCommonCommands.ServerCommands serverCommands)
            {
                TestMatchEngine = testMatchEngine;
                EventBus = eventBus;
                ServerCommands = serverCommands;
            }
        }

        private readonly Context _context;

        public ServerCommandsProcessor(Context context)
        {
            _context = context;

            _context.ServerCommands.SendState.Subscribe(ApplySyncState);
            _context.ServerCommands.StartWaveSpawn.Subscribe(StartWaveSpawn);
            _context.ServerCommands.BroadcastStateCheckSum.Subscribe(BroadcastStateCheckSum);
        }

        private void ApplySyncState(MatchState matchState)
        {
            Hashtable applySyncStateProperties = new Hashtable
            {
                {PhotonEventsConstants.SyncState.MatchStateParam, MatchState.ToHashtable(matchState)},
                {PhotonEventsConstants.SyncState.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value},
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.SyncState.ApplyEventId, applySyncStateProperties);
        }
        
        private void StartWaveSpawn(BuiltWaveParams builtWaveParams, int randomSeed)
        {
            Hashtable startWaveSpawnProperties = new Hashtable
            {
                {PhotonEventsConstants.StartWaveSpawn.DurationParam, builtWaveParams.Duration},
                {PhotonEventsConstants.StartWaveSpawn.PauseBeforeWaveParam, builtWaveParams.PauseBeforeWave},
                
                {PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsIds, builtWaveParams.Player1MobsIdsNetwork},
                {PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsDelays, builtWaveParams.Player1MobsDelaysNetwork},
                {PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsPaths, builtWaveParams.Player1MobsPathsNetwork},
                
                {PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsIds, builtWaveParams.Player2MobsIdsNetwork},
                {PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsDelays, builtWaveParams.Player2MobsDelaysNetwork},
                {PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsPaths, builtWaveParams.Player2MobsPathsNetwork},
                {PhotonEventsConstants.StartWaveSpawn.RandomSeed, randomSeed},
                {PhotonEventsConstants.StartWaveSpawn.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.StartWaveSpawn.ApplyEventId, startWaveSpawnProperties);
        }

        private void BroadcastStateCheckSum(MatchStateCheckSum matchStateCheckSum)
        {
            Hashtable broadcastStateCheckSumProperties = new Hashtable
            {
                {PhotonEventsConstants.BroadcastStateCheckSum.Player1CheckSumParam, matchStateCheckSum.Player1CheckSum},
                {PhotonEventsConstants.BroadcastStateCheckSum.Player2CheckSumParam, matchStateCheckSum.Player2CheckSum},
                {PhotonEventsConstants.BroadcastStateCheckSum.TimeParam, matchStateCheckSum.EngineFrame}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.BroadcastStateCheckSum.ApplyEventId, broadcastStateCheckSumProperties);
        }
    }
}