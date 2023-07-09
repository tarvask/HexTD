using Tools;

namespace Match.State.CheckSum
{
    public class MatchStateCheckSumComputerController : BaseDisposable
    {
        private const MatchStateCheckSumComputeTacticType CheckSumComputeTacticType = MatchStateCheckSumComputeTacticType.FieldObjectsCount;
        private const byte CheckSumHistorySize = 20;
        
        private readonly MatchStateCheckSumComputeTacticCoordinator _tacticCoordinator;
        private readonly MatchStateCheckSumHistoryHolder _historyHolder;
        
        public MatchStateCheckSum LastCheckSum => _historyHolder.LastCheckSum;

        public MatchStateCheckSumComputerController()
        {
            _tacticCoordinator = AddDisposable(new MatchStateCheckSumComputeTacticCoordinator(CheckSumComputeTacticType));
            _historyHolder = AddDisposable(new MatchStateCheckSumHistoryHolder(CheckSumHistorySize));
        }

        public void UpdateCheckSumHistory(int currentEngineFrame, in MatchState matchState)
        {
            MatchStateCheckSum checkSum = _tacticCoordinator.GetMatchCheckSum(currentEngineFrame, matchState);
            _historyHolder.AddCheckSumToHistory(checkSum);
        }

        public MatchStateCheckSumFindingResultType TryGetCheckSumForEngineFrame(int engineFrame, out MatchStateCheckSum checkSum)
        {
            return _historyHolder.TryGetCheckSumForEngineFrame(engineFrame, out checkSum);
        }
    }
}