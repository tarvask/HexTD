using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match.State;

namespace Match.Commands.Implementations
{
    public class SyncStateApplyCommandExecutor : AbstractCommandExecutor
    {
        public SyncStateApplyCommandExecutor(Context context) : base(context) { }
        
        protected override ICommandImplementation CreateServerImplementation(Context context)
        {
            return new ServerImplementation(context);
        }

        protected override ICommandImplementation CreateClientImplementation(Context context)
        {
            return new ClientImplementation(context);
        }

        protected abstract class BaseSyncStateCommandImplementation : AbstractCommandImplementation
        {
            protected BaseSyncStateCommandImplementation(Context context) : base(context) { }

            protected struct Parameters
            {
                public MatchState MatchState { get; }
                
                public int TimeStamp { get; }

                public Parameters(MatchState matchState, int timeStamp)
                {
                    MatchState = matchState;
                    TimeStamp = timeStamp;
                }
            }

            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                MatchState matchState = MatchState.FromHashtable((Hashtable) commandParametersTable[PhotonEventsConstants.SyncState.MatchStateParam]);
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.SyncState.TimeParam];
                
                return new Parameters(matchState, timeStamp);
            }
        }

        private class ServerImplementation : BaseSyncStateCommandImplementation
        {
            public ServerImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }
        }

        private class ClientImplementation : BaseSyncStateCommandImplementation
        {
            public ClientImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // no waiting for target frame
                //await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.ApplySyncState(commandParameters.MatchState,
                    commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}