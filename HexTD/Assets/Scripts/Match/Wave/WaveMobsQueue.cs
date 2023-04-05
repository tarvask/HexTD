using System.Collections.Generic;
using Match.Wave.State;
using Tools;
using Tools.Interfaces;

namespace Match.Wave
{
    public class WaveMobsQueue : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly Queue<WaveElementDelay> _waveElements;
        private readonly float _targetWaveDuration;
        
        private float _currentWaveDuration;
        private float _targetSpawnPause;
        private float _lastSpawnTime;

        public bool NextMobReady => _currentWaveDuration >= _lastSpawnTime + _targetSpawnPause;
        public bool HasMoreMobs => _waveElements.Count > 0;
        public bool HasTimeLeft => _currentWaveDuration < _targetWaveDuration;

        public WaveMobsQueue(List<WaveElementDelay> waveElements, float targetWaveDuration)
        {
            _waveElements = new Queue<WaveElementDelay>(waveElements);
            _targetWaveDuration = targetWaveDuration;
            _currentWaveDuration = 0;
            _targetSpawnPause = 0;
            _lastSpawnTime = 0;
        }

        public WaveMobsQueue(WaveElementDelay[] waveElements, int targetWaveDuration)
        {
            _waveElements = new Queue<WaveElementDelay>(waveElements);
            _targetWaveDuration = targetWaveDuration * 0.001f;
            _currentWaveDuration = 0;
            _targetSpawnPause = 0;
            _lastSpawnTime = 0;
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            _currentWaveDuration += frameLength;
        }

        public byte GetNextMobId()
        {
            byte nextMobId = _waveElements.Dequeue().MobId;
            _lastSpawnTime = _currentWaveDuration;
            
            if (HasMoreMobs)
                _targetSpawnPause = _waveElements.Peek().Delay;

            return nextMobId;
        }
        
        public void LoadState(in WavesState.WaveState waveState)
        {
            _currentWaveDuration = waveState.CurrentWaveDuration * 0.001f;
            _targetSpawnPause = waveState.TargetSpawnPause * 0.001f;
            _lastSpawnTime = waveState.LastSpawnTime * 0.001f;
        }

        public WavesState.WaveState SaveState()
        {
            return new WavesState.WaveState(_waveElements.ToArray(),
                (int)(_targetWaveDuration * 1000),
                (int)(_currentWaveDuration * 1000),
                (int)(_targetSpawnPause * 1000),
                (int)(_lastSpawnTime * 1000));
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _waveElements.Clear();
        }
    }
}