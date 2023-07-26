using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match.State;
using Match.State.CheckSum;

namespace Match.Commands.Implementations
{
    public class BroadcastStateCheckSumCommandExecutor : AbstractCommandExecutor
    {
        public BroadcastStateCheckSumCommandExecutor(Context context) : base(context) { }

        protected override ICommandImplementation CreateServerImplementation(Context context)
        {
            return new ServerImplementation(context);
        }

        protected override ICommandImplementation CreateClientImplementation(Context context)
        {
            return new ClientImplementation(context);
        }
        
        protected abstract class BaseStartWaveSpawnCommandImplementation : AbstractCommandImplementation
        {
            protected BaseStartWaveSpawnCommandImplementation(Context context) : base(context) { }

            protected struct Parameters
            {
                public MatchStateCheckSum StateCheckSum { get; }
                public int TimeStamp { get; }

                public Parameters(MatchStateCheckSum stateCheckSum, int timeStamp)
                {
                    StateCheckSum = stateCheckSum;
                    TimeStamp = timeStamp;
                }
            }
            
            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                int player1CheckSum = (int)commandParametersTable[PhotonEventsConstants.BroadcastStateCheckSum.Player1CheckSumParam];
                int player2CheckSum = (int)commandParametersTable[PhotonEventsConstants.BroadcastStateCheckSum.Player2CheckSumParam];
                
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.StartWaveSpawn.TimeParam];
                MatchStateCheckSum stateCheckSum = new MatchStateCheckSum(timeStamp,
                    player1CheckSum, player2CheckSum);
                
                return new Parameters(stateCheckSum, timeStamp);
            }
        }
        
        private class ServerImplementation : BaseStartWaveSpawnCommandImplementation
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

        private class ClientImplementation : BaseStartWaveSpawnCommandImplementation
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
                
                // wait for target frame
                // await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.BroadcastStateCheckSum(commandParameters.StateCheckSum,
                    commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}