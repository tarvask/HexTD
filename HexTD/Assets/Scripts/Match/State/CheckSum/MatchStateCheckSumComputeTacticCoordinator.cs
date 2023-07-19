using System;
using UI.Tools;

namespace Match.State.CheckSum
{
    public class MatchStateCheckSumComputeTacticCoordinator : BaseDisposable, IMatchStateCheckSumComputeTactic
    {
        private readonly IMatchStateCheckSumComputeTactic _checkSumComputeTactic;
        
        public MatchStateCheckSumComputeTacticCoordinator(MatchStateCheckSumComputeTacticType checkSumComputeType)
        {
            _checkSumComputeTactic = AddDisposable(GetTacticByType(checkSumComputeType));
        }
        
        public MatchStateCheckSum GetMatchCheckSum(int currentEngineFrame, in MatchState matchState)
        {
            return _checkSumComputeTactic.GetMatchCheckSum(currentEngineFrame, matchState);
        }

        private AbstractMatchStateCheckSumComputeTactic GetTacticByType(
            MatchStateCheckSumComputeTacticType checkSumComputeType)
        {
            switch (checkSumComputeType)
            {
                case MatchStateCheckSumComputeTacticType.FieldObjectsCount:
                    return new FieldObjectsCountTactic();
                
                default:
                    throw new ArgumentException($"Tried to use an unknown check sum computing tactic {checkSumComputeType}");
            }
        }
    }
}