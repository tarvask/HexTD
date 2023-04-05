using System.Threading.Tasks;
using ExitGames.Client.Photon;

namespace Match.Commands.Implementations
{
    public class SyncStateRequestCommandExecutor : AbstractCommandExecutor
    {
        public SyncStateRequestCommandExecutor(Context context) : base(context) { }
        
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
                public int TimeStamp { get; }

                public Parameters(int timeStamp)
                {
                    TimeStamp = timeStamp;
                }
            }

            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.SyncState.TimeParam];
                
                return new Parameters(timeStamp);
            }
        }

        private class ServerImplementation : BaseSyncStateCommandImplementation
        {
            public ServerImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // no waiting for target frame
                //await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.RequestSyncState(commandParameters.TimeStamp);
                
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
                // do nothing
                await Task.CompletedTask;
            }
        }
    }
}