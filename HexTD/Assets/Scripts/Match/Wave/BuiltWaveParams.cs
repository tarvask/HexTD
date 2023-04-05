using System.Collections.Generic;

namespace Match.Wave
{
    public struct BuiltWaveParams
    {
        private readonly List<WaveElementDelay> _player1MobsAndDelays;
        private readonly List<WaveElementDelay> _player2MobsAndDelays;
        private readonly float _duration;
        private readonly bool _areArtifactsAvailable;
        private readonly float _pauseBeforeWave;

        public List<WaveElementDelay> Player1MobsAndDelays => _player1MobsAndDelays;
        public List<WaveElementDelay> Player2MobsAndDelays => _player2MobsAndDelays;
        public float Duration => _duration;
        public bool AreArtifactsAvailable => _areArtifactsAvailable;
        public float PauseBeforeWave => _pauseBeforeWave;
        
        // network stuff
        public byte[] Player1MobsIdsNetwork
        {
            get
            {
                byte[] mobsIds = new byte[_player1MobsAndDelays.Count];

                for (int mobIndex = 0; mobIndex < _player1MobsAndDelays.Count; mobIndex++)
                {
                    mobsIds[mobIndex] = _player1MobsAndDelays[mobIndex].MobId;
                }

                return mobsIds;
            }
        }
        
        public byte[] Player2MobsIdsNetwork
        {
            get
            {
                byte[] mobsIds = new byte[_player2MobsAndDelays.Count];

                for (int mobIndex = 0; mobIndex < _player2MobsAndDelays.Count; mobIndex++)
                {
                    mobsIds[mobIndex] = _player2MobsAndDelays[mobIndex].MobId;
                }

                return mobsIds;
            }
        }
        
        public float[] Player1MobsDelaysNetwork
        {
            get
            {
                float[] mobsDelays = new float[_player1MobsAndDelays.Count];

                for (int mobIndex = 0; mobIndex < _player1MobsAndDelays.Count; mobIndex++)
                {
                    mobsDelays[mobIndex] = _player1MobsAndDelays[mobIndex].Delay;
                }

                return mobsDelays;
            }
        }
        
        public float[] Player2MobsDelaysNetwork
        {
            get
            {
                float[] mobsDelays = new float[_player2MobsAndDelays.Count];

                for (int mobIndex = 0; mobIndex < _player2MobsAndDelays.Count; mobIndex++)
                {
                    mobsDelays[mobIndex] = _player2MobsAndDelays[mobIndex].Delay;
                }

                return mobsDelays;
            }
        }

        public BuiltWaveParams(
            List<WaveElementDelay> player1MobsAndDelaysParam, List<WaveElementDelay> player2MobsAndDelaysParam,
            float durationParam,
            bool areArtifactsAvailableParam,
            float pauseBeforeWaveParam)
        {
            _player1MobsAndDelays = player1MobsAndDelaysParam;
            _player2MobsAndDelays = player2MobsAndDelaysParam;
            _duration = durationParam;
            _areArtifactsAvailable = areArtifactsAvailableParam;
            _pauseBeforeWave = pauseBeforeWaveParam;
        }
    }
}