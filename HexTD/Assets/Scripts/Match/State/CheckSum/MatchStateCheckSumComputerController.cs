using Tools;

namespace Match.State.CheckSum
{
    public class MatchStateCheckSumComputerCoordinator : BaseDisposable
    {
        private const MatchStateCheckSumComputeTacticType CheckSumComputeTacticType = MatchStateCheckSumComputeTacticType.FieldObjectsCount;
        private const byte CheckSumHistorySize = 20;
        
        private readonly MatchStateCheckSumComputeTacticCoordinator _tacticCoordinator;
        private readonly MatchStateCheckSumHistoryHolder _historyHolder;

        public MatchStateCheckSumComputerCoordinator()
        {
            _tacticCoordinator = AddDisposable(new MatchStateCheckSumComputeTacticCoordinator(CheckSumComputeTacticType));
            _historyHolder = AddDisposable(new MatchStateCheckSumHistoryHolder(CheckSumHistorySize));
        }

        public void UpdateCheckSumHistory(int currentEngineFrame, in MatchState matchState)
        {
            MatchStateCheckSum checkSum = _tacticCoordinator.GetMatchCheckSum(currentEngineFrame, matchState);
            _historyHolder.AddCheckSumToHistory(checkSum);
        }
    }
}