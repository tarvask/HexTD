using ExitGames.Client.Photon;

namespace Match.Wave
{
    public readonly struct WavesState
    {
        private readonly int _currentWaveNumber;
        private readonly WaveStateType _state;
        private readonly int _targetPauseDuration;
        private readonly int _currentPauseDuration;
        private readonly int _spawnTimer;
        private readonly WaveState[] _player1Waves;
        private readonly WaveState[] _player2Waves;

        public int CurrentWaveNumber => _currentWaveNumber;
        public WaveStateType State => _state;
        public int TargetPauseDuration => _targetPauseDuration;
        public int CurrentPauseDuration => _currentPauseDuration;
        public int SpawnTimer => _spawnTimer;
        public WaveState[] Player1Waves => _player1Waves;
        public WaveState[] Player2Waves => _player2Waves;

        public WavesState(int currentWaveNumber, WaveStateType state,
            int targetPauseDuration, int currentPauseDuration, int spawnTimer,
            ref WaveState[] player1Waves, ref WaveState[] player2Waves)
        {
            _currentWaveNumber = currentWaveNumber;
            _state = state;
            _targetPauseDuration = targetPauseDuration;
            _currentPauseDuration = currentPauseDuration;
            _spawnTimer = spawnTimer;
            _player1Waves = player1Waves;
            _player2Waves = player2Waves;
        }

        public static WavesState FromHashtable(Hashtable wavesStateHashtable)
        {
            int currentWaveNumber = (int)wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.CurrentWaveNumberParam];
            WaveStateType state = (WaveStateType)(byte)wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.StateParam];
            int targetPauseDuration = (int)wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.TargetPauseDurationParam];
            int currentPauseDuration = (int)wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.CurrentPauseDurationParam];
            int spawnTimer = (int)wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.SpawnTimerParam];
            
            // player 1 waves
            Hashtable[] player1WavesHashtables = (Hashtable[])wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.Player1WavesParam];
            
            WaveState[] player1WavesStates = new WaveState[player1WavesHashtables.Length];

            for (int waveIndex = 0; waveIndex < player1WavesHashtables.Length; waveIndex++)
                player1WavesStates[waveIndex] = WaveState.WaveFromHashtable(player1WavesHashtables[waveIndex]);
            
            // player 2 waves
            Hashtable[] player2WavesHashtables = (Hashtable[])wavesStateHashtable[PhotonEventsConstants.SyncState.WavesState.Player2WavesParam];
            
            WaveState[] player2WavesStates = new WaveState[player2WavesHashtables.Length];

            for (int waveIndex = 0; waveIndex < player2WavesHashtables.Length; waveIndex++)
                player2WavesStates[waveIndex] = WaveState.WaveFromHashtable(player2WavesHashtables[waveIndex]);

            return new WavesState(currentWaveNumber, state, targetPauseDuration, currentPauseDuration, spawnTimer,
                ref player1WavesStates, ref player2WavesStates);
        }

        public static Hashtable ToHashtable(in WavesState wavesState)
        {
            Hashtable[] player1WavesHashtables = new Hashtable[wavesState.Player1Waves.Length];

            for (int waveIndex = 0; waveIndex < wavesState.Player1Waves.Length; waveIndex++)
            {
                player1WavesHashtables[waveIndex] = WaveState.WaveToHashtable(wavesState.Player1Waves[waveIndex]);
            }
            
            Hashtable[] player2WavesHashtables = new Hashtable[wavesState.Player2Waves.Length];
            
            for (int waveIndex = 0; waveIndex < wavesState.Player2Waves.Length; waveIndex++)
            {
                player2WavesHashtables[waveIndex] = WaveState.WaveToHashtable(wavesState.Player2Waves[waveIndex]);
            }
            
            return new Hashtable
            {
                {PhotonEventsConstants.SyncState.WavesState.CurrentWaveNumberParam, wavesState.CurrentWaveNumber},
                {PhotonEventsConstants.SyncState.WavesState.StateParam, wavesState.State},
                {PhotonEventsConstants.SyncState.WavesState.TargetPauseDurationParam, wavesState.TargetPauseDuration},
                {PhotonEventsConstants.SyncState.WavesState.CurrentPauseDurationParam, wavesState.CurrentPauseDuration},
                {PhotonEventsConstants.SyncState.WavesState.SpawnTimerParam, wavesState.SpawnTimer},
                {PhotonEventsConstants.SyncState.WavesState.Player1WavesParam, player1WavesHashtables},
                {PhotonEventsConstants.SyncState.WavesState.Player2WavesParam, player2WavesHashtables},
            };
        }

        public readonly struct WaveState
        {
            private readonly WaveElementDelayAndPath[] _waveElements;
            private readonly int _targetWaveDuration;
            private readonly int _currentWaveDuration;
            private readonly int _targetSpawnPause;
            private readonly int _lastSpawnTime;

            public WaveElementDelayAndPath[] WaveElements => _waveElements;
            public int TargetWaveDuration => _targetWaveDuration;
            public int CurrentWaveDuration => _currentWaveDuration;
            public int TargetSpawnPause => _targetSpawnPause;
            public int LastSpawnTime => _lastSpawnTime;

            public WaveState(WaveElementDelayAndPath[] waveElements, int targetWaveDuration, int currentWaveDuration,
                int targetSpawnPause, int lastSpawnTime)
            {
                _waveElements = waveElements;
                _targetWaveDuration = targetWaveDuration;
                _currentWaveDuration = currentWaveDuration;
                _targetSpawnPause = targetSpawnPause;
                _lastSpawnTime = lastSpawnTime;
            }
            
            public static WaveState WaveFromHashtable(Hashtable waveHashtable)
            {
                Hashtable[] waveElementsHashtables = (Hashtable[])waveHashtable[PhotonEventsConstants.SyncState.WavesState.WaveState.WaveElementsParam];
            
                WaveElementDelayAndPath[] waveElements = new WaveElementDelayAndPath[waveElementsHashtables.Length];

                for (int waveElementIndex = 0; waveElementIndex < waveElementsHashtables.Length; waveElementIndex++)
                    waveElements[waveElementIndex] = WaveElementFromHashtable(waveElementsHashtables[waveElementIndex]);

                int targetWaveDuration = (int)waveHashtable[PhotonEventsConstants.SyncState.WavesState.WaveState.TargetWaveDurationParam];
                int currentWaveDuration = (int)waveHashtable[PhotonEventsConstants.SyncState.WavesState.WaveState.CurrentWaveDuration];
                int targetSpawnPause = (int) waveHashtable[PhotonEventsConstants.SyncState.WavesState.WaveState.TargetSpawnPause];
                int lastSpawnTime = (int) waveHashtable[PhotonEventsConstants.SyncState.WavesState.WaveState.LastSpawnTime];
                
                return new WaveState(waveElements, targetWaveDuration, currentWaveDuration, targetSpawnPause, lastSpawnTime);
            }

            public static Hashtable WaveToHashtable(in WaveState waveState)
            {
                Hashtable[] waveElementsHashtables = new Hashtable[waveState.WaveElements.Length];

                for (int elementId = 0; elementId < waveElementsHashtables.Length; elementId++)
                {
                    waveElementsHashtables[elementId] = WaveElementToHashtable(waveState.WaveElements[elementId]);
                }

                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.WavesState.WaveState.WaveElementsParam, waveElementsHashtables},
                    {PhotonEventsConstants.SyncState.WavesState.WaveState.TargetWaveDurationParam, waveState.TargetWaveDuration},
                    {PhotonEventsConstants.SyncState.WavesState.WaveState.CurrentWaveDuration, waveState.CurrentWaveDuration},
                    {PhotonEventsConstants.SyncState.WavesState.WaveState.TargetSpawnPause, waveState.TargetSpawnPause},
                    {PhotonEventsConstants.SyncState.WavesState.WaveState.LastSpawnTime, waveState.LastSpawnTime},
                };
            }

            private static WaveElementDelayAndPath WaveElementFromHashtable(Hashtable waveElementHashtable)
            {
                byte mobId = (byte)waveElementHashtable[PhotonEventsConstants.SyncState.WavesState.WaveElementState.MobIdParam];
                int delay = (int)waveElementHashtable[PhotonEventsConstants.SyncState.WavesState.WaveElementState.DelayParam];
                byte pathId = (byte)waveElementHashtable[PhotonEventsConstants.SyncState.WavesState.WaveElementState.PathParam];
                
                return new WaveElementDelayAndPath(mobId, delay * 0.001f, pathId);
            }
            
            private static Hashtable WaveElementToHashtable(WaveElementDelayAndPath waveElement)
            {
                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.WavesState.WaveElementState.MobIdParam, waveElement.MobId},
                    {PhotonEventsConstants.SyncState.WavesState.WaveElementState.DelayParam, (int)(waveElement.Delay * 1000)},
                    {PhotonEventsConstants.SyncState.WavesState.WaveElementState.PathParam, waveElement.PathId},
                };
            }
        }
    }
}