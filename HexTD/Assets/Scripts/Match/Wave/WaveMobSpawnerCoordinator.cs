using System;
using Match.Commands;
using Match.Field;
using Match.Field.Mob;
using Match.Wave.State;
using Services;
using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Wave
{
    public class WaveMobSpawnerCoordinator : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public ConfigsRetriever ConfigsRetriever { get; }
            public FieldConfig FieldConfig { get; }
            public MatchCommonCommands.IncomingGeneralCommands IncomingGeneralGeneralCommands { get; }
            public MatchCommands.IncomingCommands Player1IncomingCommands { get; }
            public MatchCommands.IncomingCommands Player2IncomingCommands { get; }
            public MatchCommonCommands.ServerCommands ServerCommands { get; }
            public WaveParametersStrict[] Waves { get; }
            public bool IsMultiPlayer { get; }
            public IReadOnlyReactiveProperty<NetworkRoles> OurNetworkRoleReactiveProperty { get; }

            public ReactiveCommand<float> MatchStartedReactiveCommand { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public ReactiveCommand WaveEndedReactiveCommand { get; }
            public ReactiveCommand<float> ArtifactChoosingStartedReactiveCommand { get; }
            public ReactiveCommand<float> BetweenWavesPlanningStartedReactiveCommand { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand<MobConfig> SpawnPlayer1MobReactiveCommand { get; }
            public ReactiveCommand<MobConfig> SpawnPlayer2MobReactiveCommand { get; }
            public IReadOnlyReactiveProperty<bool> HasMobsOnEnemyField { get; }
            public IReadOnlyReactiveProperty<bool> HasMobsOnOurField { get; }

            public Context(
                ConfigsRetriever configsRetriever,
                FieldConfig fieldConfig,
                MatchCommonCommands.IncomingGeneralCommands incomingGeneralCommands,
                MatchCommands.IncomingCommands player1IncomingCommands,
                MatchCommands.IncomingCommands player2IncomingCommands,
                MatchCommonCommands.ServerCommands serverCommands,
                WaveParametersStrict[] waves,
                bool isMultiPlayer,
                IReadOnlyReactiveProperty<NetworkRoles> ourNetworkRoleReactiveProperty,

                ReactiveCommand<float> matchStartedReactiveCommand,
                ReactiveCommand<float> waveStartedReactiveCommand,
                ReactiveCommand waveEndedReactiveCommand,
                ReactiveCommand<float> artifactChoosingStartedReactiveCommand,
                ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand<MobConfig> spawnPlayer1MobReactiveCommand,
                ReactiveCommand<MobConfig> spawnPlayer2MobReactiveCommand,
                IReadOnlyReactiveProperty<bool> hasMobsOnEnemyField,
                IReadOnlyReactiveProperty<bool> hasMobsOnOurField)
            {
                ConfigsRetriever = configsRetriever;
                FieldConfig = fieldConfig;
                IncomingGeneralGeneralCommands = incomingGeneralCommands;
                Player1IncomingCommands = player1IncomingCommands;
                Player2IncomingCommands = player2IncomingCommands;
                ServerCommands = serverCommands;
                Waves = waves;
                IsMultiPlayer = isMultiPlayer;
                OurNetworkRoleReactiveProperty = ourNetworkRoleReactiveProperty;

                MatchStartedReactiveCommand = matchStartedReactiveCommand;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                WaveEndedReactiveCommand = waveEndedReactiveCommand;
                ArtifactChoosingStartedReactiveCommand = artifactChoosingStartedReactiveCommand;
                BetweenWavesPlanningStartedReactiveCommand = betweenWavesPlanningStartedReactiveCommand;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                SpawnPlayer1MobReactiveCommand = spawnPlayer1MobReactiveCommand;
                SpawnPlayer2MobReactiveCommand = spawnPlayer2MobReactiveCommand;
                HasMobsOnEnemyField = hasMobsOnEnemyField;
                HasMobsOnOurField = hasMobsOnOurField;
            }
        }
        
        public const byte MaxMobsInWave = 100;
        
        private readonly Context _context;

        private readonly WaveMobSpawnerBaseNoReinforcements _serverImplementation;
        private readonly WaveMobSpawnerBaseNoReinforcements _clientImplementation;
        private WaveMobSpawnerBaseNoReinforcements _currentImplementation;

        public int CurrentWaveNumber => _currentImplementation.CurrentWaveNumber;

        public WaveMobSpawnerCoordinator(Context context)
        {
            _context = context;

            WaveMobSpawnerBaseNoReinforcements.Context waveMobSpawnerImplementationContext = new WaveMobSpawnerBaseNoReinforcements.Context(
                _context.ConfigsRetriever, _context.FieldConfig,
                _context.IncomingGeneralGeneralCommands,
                _context.Player1IncomingCommands, _context.Player2IncomingCommands, _context.ServerCommands,
                _context.Waves,
                _context.IsMultiPlayer,
                
                _context.MatchStartedReactiveCommand,
                _context.WaveStartedReactiveCommand, _context.WaveEndedReactiveCommand,
                _context.ArtifactChoosingStartedReactiveCommand,
                _context.BetweenWavesPlanningStartedReactiveCommand,
                _context.WaveNumberChangedReactiveCommand,
                _context.SpawnPlayer1MobReactiveCommand, _context.SpawnPlayer2MobReactiveCommand,
                _context.HasMobsOnEnemyField, _context.HasMobsOnOurField);
            _serverImplementation = new WaveMobSpawnerServerNoReinforcements(waveMobSpawnerImplementationContext);
            _clientImplementation = new WaveMobSpawnerClientNoReinforcements(waveMobSpawnerImplementationContext);

            // subscriptions
            _context.OurNetworkRoleReactiveProperty.Subscribe(UpdateRole);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            _currentImplementation.OuterLogicUpdate(frameLength);
        }

        public void LoadState(WavesState wavesState)
        {
            _currentImplementation.LoadState(wavesState);
        }

        private void UpdateRole(NetworkRoles newRole)
        {
            // save state from current implementation and load it to new
            if (_currentImplementation != null)
                MigrateState(newRole);

            switch (newRole)
            {
                case NetworkRoles.Client:
                    _currentImplementation = _clientImplementation;
                    break;
                case NetworkRoles.Server:
                    _currentImplementation = _serverImplementation;
                    break;
                default:
                    throw new ArgumentException("Bad network role");
            }
        }

        private void MigrateState(NetworkRoles newRole)
        {
            WavesState currentState = GetWaveState();
            
            switch (newRole)
            {
                case NetworkRoles.Client:
                    _clientImplementation.LoadState(currentState);
                    break;
                case NetworkRoles.Server:
                    _serverImplementation.LoadState(currentState);
                    break;
                default:
                    throw new ArgumentException("Bad network role");
            }
        }

        public WavesState GetWaveState()
        {
            return _currentImplementation.SaveState();
        }
    }
}