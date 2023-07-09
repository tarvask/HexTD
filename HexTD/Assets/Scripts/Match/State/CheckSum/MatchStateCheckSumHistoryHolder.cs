using System.Collections.Generic;
using Tools;

namespace Match.State.CheckSum
{
    public class MatchStateCheckSumHistoryHolder : BaseDisposable
    {
        private readonly byte _checkSumHistorySize;
        private readonly Queue<MatchStateCheckSum> _checkSumHistory;
        private MatchStateCheckSum _lastCheckSum;
        private int _minEngineFrameInHistory;
        private int _maxEngineFrameInHistory;

        public MatchStateCheckSum LastCheckSum => _lastCheckSum;

        public MatchStateCheckSumHistoryHolder(byte checkSumHistorySize)
        {
            _checkSumHistorySize = checkSumHistorySize;
            _checkSumHistory = new Queue<MatchStateCheckSum>(checkSumHistorySize);
            _lastCheckSum = default;
            _minEngineFrameInHistory = 0;
            _maxEngineFrameInHistory = 0;
        }
        
        public void AddCheckSumToHistory(in MatchStateCheckSum checkSum)
        {
            if (_checkSumHistory.Count == _checkSumHistorySize)
                _checkSumHistory.Dequeue();
            
            _checkSumHistory.Enqueue(checkSum);
            _lastCheckSum = checkSum;
            _minEngineFrameInHistory = _checkSumHistory.Peek().EngineFrame;
            _maxEngineFrameInHistory = checkSum.EngineFrame;
        }

        public MatchStateCheckSumFindingResultType TryGetCheckSumForEngineFrame(int engineFrame, out MatchStateCheckSum checkSum)
        {
            checkSum = default;
            
            if (IsEngineFrameTooOldForHistoryBounds(engineFrame))
                return MatchStateCheckSumFindingResultType.TooOld;
            
            if (IsEngineFrameTooOldForHistoryBounds(engineFrame))
                return MatchStateCheckSumFindingResultType.TooNew;

            foreach (MatchStateCheckSum matchStateCheckSum in _checkSumHistory)
            {
                if (matchStateCheckSum.EngineFrame == engineFrame)
                {
                    checkSum = matchStateCheckSum;
                    return MatchStateCheckSumFindingResultType.ExistInRange;
                }
            }

            return MatchStateCheckSumFindingResultType.NotExistInRange;
        }

        private bool IsEngineFrameTooOldForHistoryBounds(int engineFrame)
        {
            return engineFrame < _minEngineFrameInHistory;
        }
        
        private bool IsEngineFrameTooNewForHistoryBounds(int engineFrame)
        {
            return _maxEngineFrameInHistory < engineFrame;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _checkSumHistory.Clear();
        }
    }
}