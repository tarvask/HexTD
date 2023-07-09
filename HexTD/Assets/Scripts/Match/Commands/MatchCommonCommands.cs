using Match.State;
using Match.State.CheckSum;
using Match.Wave;
using Tools;

namespace Match.Commands
{
    // commands, that are simultaneously used by both fields
    public class MatchCommonCommands
    {
        public class ServerCommands : BaseDisposable
        {
            public ObservableEvent<MatchState> SendState { get; }
            public ObservableEvent<BuiltWaveParams, int> StartWaveSpawn { get; }
            public ObservableEvent<MatchStateCheckSum> BroadcastStateCheckSum { get; }

            public ServerCommands()
            {
                SendState = AddDisposable(new ObservableEvent<MatchState>());
                StartWaveSpawn = AddDisposable(new ObservableEvent<BuiltWaveParams, int>());
                BroadcastStateCheckSum = AddDisposable(new ObservableEvent<MatchStateCheckSum>());
            }
        }

        public class IncomingGeneralCommands : BaseDisposable
        {
            public ObservableEvent RequestSyncState { get; }
            public ObservableEvent<MatchState, int> ApplySyncState { get; }
            public ObservableEvent<BuiltWaveParams, int> StartWaveSpawn { get; }
            public ObservableEvent<MatchStateCheckSum> BroadcastStateCheckSum { get; }

            public IncomingGeneralCommands()
            {
                RequestSyncState = AddDisposable(new ObservableEvent());
                ApplySyncState = AddDisposable(new ObservableEvent<MatchState, int>());
                StartWaveSpawn = AddDisposable(new ObservableEvent<BuiltWaveParams, int>());
                BroadcastStateCheckSum = AddDisposable(new ObservableEvent<MatchStateCheckSum>());
            }
        }
        
        public ServerCommands Server { get; }
        public IncomingGeneralCommands IncomingGeneral { get; }

        public MatchCommonCommands(ServerCommands serverCommands, IncomingGeneralCommands incomingGeneral)
        {
            Server = serverCommands;
            IncomingGeneral = incomingGeneral;
        }
    }
}