using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match.Wave;

namespace Match.Commands.Implementations
{
    public class StartWaveSpawnCommandExecutor : AbstractCommandExecutor
    {
        public StartWaveSpawnCommandExecutor(Context context) : base(context) { }

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
                public BuiltWaveParams WaveParams { get; }
                public int RandomSeed { get; }
                public int TimeStamp { get; }

                public Parameters(BuiltWaveParams waveParams, int randomSeed, int timeStamp)
                {
                    WaveParams = waveParams;
                    RandomSeed = randomSeed;
                    TimeStamp = timeStamp;
                }
            }
            
            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                float waveDuration = (float)commandParametersTable[PhotonEventsConstants.StartWaveSpawn.DurationParam];
                float pauseBeforeWave = (float) commandParametersTable[PhotonEventsConstants.StartWaveSpawn.PauseBeforeWaveParam];

                // mobs and delays
                // player1
                byte[] mobsIds = (byte[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsIds];
                float[] mobsDelays = (float[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsDelays];
                byte[] mobsPaths = (byte[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player1WaveMobsPaths];
                List<WaveElementDelayAndPath> player1MobsAndDelays = new List<WaveElementDelayAndPath>(mobsIds.Length);

                for (int mobIndex = 0; mobIndex < mobsIds.Length; mobIndex++)
                {
                    player1MobsAndDelays.Add(new WaveElementDelayAndPath(
                        mobsIds[mobIndex],
                        mobsDelays[mobIndex],
                        mobsPaths[mobIndex]));
                }
                
                // player2
                mobsIds = (byte[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsIds];
                mobsDelays = (float[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsDelays];
                mobsPaths = (byte[])commandParametersTable[PhotonEventsConstants.StartWaveSpawn.Player2WaveMobsPaths];
                List<WaveElementDelayAndPath> player2MobsAndDelays = new List<WaveElementDelayAndPath>(mobsIds.Length);

                for (int mobIndex = 0; mobIndex < mobsIds.Length; mobIndex++)
                {
                    player2MobsAndDelays.Add(new WaveElementDelayAndPath(
                        mobsIds[mobIndex],
                        mobsDelays[mobIndex],
                        mobsPaths[mobIndex]));
                }

                int randomSeed = (int)commandParametersTable[PhotonEventsConstants.StartWaveSpawn.RandomSeed];
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.StartWaveSpawn.TimeParam];
                BuiltWaveParams spawnWaveParams = new BuiltWaveParams(
                    player1MobsAndDelays, player2MobsAndDelays,
                    waveDuration, pauseBeforeWave);
                
                return new Parameters(spawnWaveParams, randomSeed, timeStamp);
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
                await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.StartWaveSpawn(commandParameters.WaveParams,
                    commandParameters.RandomSeed, commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}