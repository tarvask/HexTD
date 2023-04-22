using System.Collections.Generic;
using Match.Field.Mob;
using Tools;
using Tools.Interfaces;

namespace Match.Wave
{
    public class WaveMobsQueue : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly Queue<WaveElementDelayAndPath> _waveElements;
        private readonly float _targetWaveDuration;
        
        private float _currentWaveDuration;
        private float _targetSpawnPause;
        private float _lastSpawnTime;

        public bool NextMobReady => _currentWaveDuration >= _lastSpawnTime + _targetSpawnPause;
        public bool HasMoreMobs => _waveElements.Count > 0;
        public bool HasTimeLeft => _currentWaveDuration < _targetWaveDuration;

        public WaveMobsQueue(List<WaveElementDelayAndPath> waveElements, float targetWaveDuration)
        {
            _waveElements = new Queue<WaveElementDelayAndPath>(waveElements);
            _targetWaveDuration = targetWaveDuration;
            _currentWaveDuration = 0;
            _targetSpawnPause = 0;
            _lastSpawnTime = 0;
        }

        public WaveMobsQueue(WaveElementDelayAndPath[] waveElements, int targetWaveDuration)
        {
            _waveElements = new Queue<WaveElementDelayAndPath>(waveElements);
            _targetWaveDuration = targetWaveDuration * 0.001f;
            _currentWaveDuration = 0;
            _targetSpawnPause = 0;
            _lastSpawnTime = 0;
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            _currentWaveDuration += frameLength;
        }

        public MobWithPath GetNextElement()
        {
            WaveElementDelayAndPath nextElement = _waveElements.Dequeue();
            _lastSpawnTime = _currentWaveDuration;
            
            if (HasMoreMobs)
                _targetSpawnPause = _waveElements.Peek().Delay;

            return new MobWithPath(nextElement.MobId, nextElement.PathId);
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