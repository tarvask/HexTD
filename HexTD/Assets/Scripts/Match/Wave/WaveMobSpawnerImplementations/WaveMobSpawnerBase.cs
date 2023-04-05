using System;
using System.Collections.Generic;
using Match.Commands;
using Match.Field;
using Match.Field.Mob;
using Match.Wave.State;
using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Wave.WaveMobSpawnerImplementations
{
    public abstract class WaveMobSpawnerBase : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public MobConfigRetriever MobConfigRetriever { get; }
            public FieldConfig FieldConfig { get; }
            public MatchCommonCommands.IncomingGeneralCommands IncomingGeneralGeneralCommands { get; }
            public MatchCommands.IncomingCommands Player1IncomingCommands { get; }
            public MatchCommands.IncomingCommands Player2IncomingCommands { get; }
            public MatchCommonCommands.ServerCommands ServerCommands { get; }
            public WaveParams[] Waves { get; }
            public bool IsMultiPlayer { get; }

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

            public Context(MobConfigRetriever mobConfigRetriever,
                FieldConfig fieldConfig,
                MatchCommonCommands.IncomingGeneralCommands incomingGeneralCommands,
                MatchCommands.IncomingCommands player1IncomingCommands,
                MatchCommands.IncomingCommands player2IncomingCommands,
                MatchCommonCommands.ServerCommands serverCommands,
                WaveParams[] waves,
                bool isMultiPlayer,

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
                MobConfigRetriever = mobConfigRetriever;
                FieldConfig = fieldConfig;
                IncomingGeneralGeneralCommands = incomingGeneralCommands;
                Player1IncomingCommands = player1IncomingCommands;
                Player2IncomingCommands = player2IncomingCommands;
                ServerCommands = serverCommands;
                Waves = waves;
                IsMultiPlayer = isMultiPlayer;

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
        
        protected readonly Context _context;
        private int _currentWaveNumber;
        private WaveStateType _state;
        private float _targetPauseDuration;
        private float _currentPauseDuration;

        private readonly Queue<WaveMobsQueue> _currentPlayer1Waves;
        private readonly Queue<WaveMobsQueue> _currentPlayer2Waves;

        public int CurrentWaveNumber => _currentWaveNumber;

        protected WaveMobSpawnerBase(Context context)
        {
            _context = context;
            
            RoleSpecialConstructorActions();
            _currentWaveNumber = -1;
            _currentPlayer1Waves = new Queue<WaveMobsQueue>(_context.FieldConfig.MaxOverlappingWaves);
            _currentPlayer2Waves = new Queue<WaveMobsQueue>(_context.FieldConfig.MaxOverlappingWaves);
            SetState(WaveStateType.Loading);
            _context.MatchStartedReactiveCommand.Execute(_context.FieldConfig.MatchInfoShowDuration);
        }

        protected abstract void RoleSpecialConstructorActions();
        
        private void SetState(WaveStateType newState)
        {
            _state = newState;
        }

        public void LoadState(WavesState wavesState)
        {
            _currentWaveNumber = wavesState.CurrentWaveNumber;
            _state = wavesState.State;
            // convert from int in milliseconds to float
            _targetPauseDuration = wavesState.TargetPauseDuration * 0.001f;
            _currentPauseDuration = wavesState.CurrentPauseDuration * 0.001f;

            // player 1 waves
            foreach (WaveMobsQueue mobsQueue in _currentPlayer1Waves)
                mobsQueue.Dispose();

            _currentPlayer1Waves.Clear();

            foreach (WavesState.WaveState waveState in wavesState.Player1Waves)
            {
                WaveMobsQueue wave = new WaveMobsQueue(waveState.WaveElements, waveState.TargetWaveDuration);
                wave.LoadState(waveState);
                _currentPlayer1Waves.Enqueue(wave);
            }
            
            // player 2 waves
            foreach (WaveMobsQueue mobsQueue in _currentPlayer2Waves)
                mobsQueue.Dispose();

            _currentPlayer2Waves.Clear();

            foreach (WavesState.WaveState waveState in wavesState.Player2Waves)
            {
                WaveMobsQueue wave = new WaveMobsQueue(waveState.WaveElements, waveState.TargetWaveDuration);
                wave.LoadState(waveState);
                _currentPlayer2Waves.Enqueue(wave);
            }
            
            _context.WaveNumberChangedReactiveCommand.Execute(_currentWaveNumber + 1);
        }

        public WavesState SaveState()
        {
            int waveIndex = 0;
            WavesState.WaveState[] player1Waves = new WavesState.WaveState[_currentPlayer1Waves.Count];

            foreach (WaveMobsQueue wave in _currentPlayer1Waves)
                player1Waves[waveIndex++] = wave.SaveState();

            waveIndex = 0;
            WavesState.WaveState[] player2Waves = new WavesState.WaveState[_currentPlayer1Waves.Count];
            
            foreach (WaveMobsQueue wave in _currentPlayer2Waves)
                player2Waves[waveIndex++] = wave.SaveState();
            
            return new WavesState(_currentWaveNumber, _state,
                // convert from float to int in milliseconds
                (int)(_targetPauseDuration * 1000), (int) (_currentPauseDuration * 1000),
                ref player1Waves, ref player2Waves);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            switch (_state)
            {
                case WaveStateType.Loading:
                    UpdateInLoading();
                    break;
                case WaveStateType.BetweenWavesTechnicalPause:
                    UpdateInTechnicalPause(frameLength);
                    break;
                case WaveStateType.ArtifactChoosing:
                    UpdateInArtifactChoosing(frameLength);
                    break;
                case WaveStateType.BetweenWavesPlanning:
                    UpdateInBetweenWavesPlanning(frameLength);
                    break;
                case WaveStateType.Spawning:
                    UpdateInSpawning(frameLength);
                    break;
                default:
                    throw new ArgumentException("Update in unknown state");
            }
        }

        private void UpdateInLoading()
        {
            NextWave();
        }

        private void UpdateInTechnicalPause(float frameLength)
        {
            _currentPauseDuration += frameLength;

            if (_currentPauseDuration >= _context.FieldConfig.TechnicalPauseBetweenWavesDuration)
            {
                NextWave();
                _currentPauseDuration = 0;
            }
        }

        private void UpdateInBetweenWavesPlanning(float frameLength)
        {
            _currentPauseDuration += frameLength;

            if (_currentPauseDuration >= _targetPauseDuration)
            {
                SetState(WaveStateType.Spawning);
                _context.WaveStartedReactiveCommand.Execute(_context.FieldConfig.WaveInfoShowDuration);
                _currentPauseDuration = 0;
            }
        }

        private void UpdateInArtifactChoosing(float frameLength)
        {
            _currentPauseDuration += frameLength;

            if (_currentPauseDuration >= _context.FieldConfig.TargetArtifactChoosingDuration)
            {
                SetState(WaveStateType.BetweenWavesPlanning);
                _currentPauseDuration = 0;
            }
        }

        private void UpdateInSpawning(float frameLength)
        {
            if (_currentPlayer1Waves.Count == 0 && _currentPlayer2Waves.Count == 0)
                return;

            bool callNextWave = UpdateCurrentWaves(_currentPlayer1Waves, _context.SpawnPlayer1MobReactiveCommand, frameLength);
            callNextWave &= UpdateCurrentWaves(_currentPlayer2Waves, _context.SpawnPlayer2MobReactiveCommand, frameLength, _context.IsMultiPlayer);
            
            if (callNextWave)
            {
                _context.WaveEndedReactiveCommand.Execute();
                SetState(WaveStateType.BetweenWavesTechnicalPause);
            }
        }
        
        private bool UpdateCurrentWaves(Queue<WaveMobsQueue> currentWaves,
            ReactiveCommand<MobConfig> spawnPlayerMobReactiveCommand, float frameLength, bool canSpawnForPlayer = true)
        {
            int activeWaveIndex = 0;
            bool callNextWave = false;
            
            foreach (WaveMobsQueue waveMobsQueue in currentWaves)
            {
                waveMobsQueue.OuterLogicUpdate(frameLength);

                while (waveMobsQueue.NextMobReady && waveMobsQueue.HasMoreMobs)
                {
                    Spawn(waveMobsQueue.GetNextMobId(), spawnPlayerMobReactiveCommand, canSpawnForPlayer);
                }
                
                // last spawning wave
                // time ended or all mobs spawned and killed
                if (activeWaveIndex == currentWaves.Count - 1
                    && (!waveMobsQueue.HasTimeLeft
                        ||
                        !waveMobsQueue.HasMoreMobs && !_context.HasMobsOnEnemyField.Value && !_context.HasMobsOnOurField.Value))
                {
                    callNextWave = true;
                }

                activeWaveIndex++;
            }
            
            return callNextWave;
        }

        protected abstract void NextWave();
        
        // use builtPlayer1WaveParams, if no branching for player1/player2 is needed
        protected void StartWave(BuiltWaveParams builtWaveParams, int randomSeed)
        {
            Randomizer.InitState(randomSeed);
            _currentWaveNumber = _currentWaveNumber + 1;
            _targetPauseDuration = builtWaveParams.PauseBeforeWave;
            
            _currentPlayer1Waves.Enqueue(new WaveMobsQueue(builtWaveParams.Player1MobsAndDelays, builtWaveParams.Duration));
            _currentPlayer2Waves.Enqueue(new WaveMobsQueue(builtWaveParams.Player2MobsAndDelays, builtWaveParams.Duration));
            
            // send +1 to avoid counting from 0
            _context.WaveNumberChangedReactiveCommand.Execute(_currentWaveNumber + 1);

            SetState(WaveStateType.BetweenWavesPlanning);
            _context.BetweenWavesPlanningStartedReactiveCommand.Execute(_targetPauseDuration);
        }
        
        private void Spawn(byte mobId, ReactiveCommand<MobConfig> spawnPlayerMobReactiveCommand, bool canSpawn)
        {
            if (canSpawn)
                spawnPlayerMobReactiveCommand.Execute(_context.MobConfigRetriever.GetMobById(mobId));
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            // player 1 waves
            foreach (WaveMobsQueue waveMobsQueue in _currentPlayer1Waves)
            {
                waveMobsQueue.Dispose();
            }
            
            _currentPlayer1Waves.Clear();
            
            // player 2 waves
            foreach (WaveMobsQueue waveMobsQueue in _currentPlayer2Waves)
            {
                waveMobsQueue.Dispose();
            }
            
            _currentPlayer2Waves.Clear();
        }
    }
}