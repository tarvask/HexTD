using System.Collections.Generic;

namespace Match.Wave
{
    public readonly struct BuiltWaveParams
    {
        private readonly List<WaveElementDelayAndPath> _player1MobsWithDelaysAndPaths;
        private readonly List<WaveElementDelayAndPath> _player2MobsWithDelaysAndPaths;
        private readonly float _duration;
        private readonly float _pauseBeforeWave;

        public List<WaveElementDelayAndPath> Player1MobsWithDelaysAndPaths => _player1MobsWithDelaysAndPaths;
        public List<WaveElementDelayAndPath> Player2MobsWithDelaysAndPaths => _player2MobsWithDelaysAndPaths;
        public float Duration => _duration;
        public float PauseBeforeWave => _pauseBeforeWave;
        
        // network stuff
        public byte[] Player1MobsIdsNetwork
        {
            get
            {
                byte[] mobsIds = new byte[_player1MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player1MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsIds[mobIndex] = _player1MobsWithDelaysAndPaths[mobIndex].MobId;
                }

                return mobsIds;
            }
        }
        
        public byte[] Player2MobsIdsNetwork
        {
            get
            {
                byte[] mobsIds = new byte[_player2MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player2MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsIds[mobIndex] = _player2MobsWithDelaysAndPaths[mobIndex].MobId;
                }

                return mobsIds;
            }
        }
        
        public float[] Player1MobsDelaysNetwork
        {
            get
            {
                float[] mobsDelays = new float[_player1MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player1MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsDelays[mobIndex] = _player1MobsWithDelaysAndPaths[mobIndex].Delay;
                }

                return mobsDelays;
            }
        }
        
        public float[] Player2MobsDelaysNetwork
        {
            get
            {
                float[] mobsDelays = new float[_player2MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player2MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsDelays[mobIndex] = _player2MobsWithDelaysAndPaths[mobIndex].Delay;
                }

                return mobsDelays;
            }
        }
        
        public byte[] Player1MobsPathsNetwork
        {
            get
            {
                byte[] mobsPaths = new byte[_player1MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player1MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsPaths[mobIndex] = _player1MobsWithDelaysAndPaths[mobIndex].PathId;
                }

                return mobsPaths;
            }
        }
        
        public byte[] Player2MobsPathsNetwork
        {
            get
            {
                byte[] mobsPaths = new byte[_player2MobsWithDelaysAndPaths.Count];

                for (int mobIndex = 0; mobIndex < _player2MobsWithDelaysAndPaths.Count; mobIndex++)
                {
                    mobsPaths[mobIndex] = _player2MobsWithDelaysAndPaths[mobIndex].PathId;
                }

                return mobsPaths;
            }
        }

        public BuiltWaveParams(
            List<WaveElementDelayAndPath> player1MobsWithDelaysAndPathsParam, List<WaveElementDelayAndPath> player2MobsWithDelaysAndPathsParam,
            float durationParam,
            float pauseBeforeWaveParam)
        {
            _player1MobsWithDelaysAndPaths = player1MobsWithDelaysAndPathsParam;
            _player2MobsWithDelaysAndPaths = player2MobsWithDelaysAndPathsParam;
            _duration = durationParam;
            _pauseBeforeWave = pauseBeforeWaveParam;
        }
    }
}