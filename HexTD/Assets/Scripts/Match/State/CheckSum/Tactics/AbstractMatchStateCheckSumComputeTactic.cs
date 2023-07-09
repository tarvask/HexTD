using Match.Field.State;
using UI.Tools;

namespace Match.State.CheckSum
{
    public abstract class AbstractMatchStateCheckSumComputeTactic : BaseDisposable, IMatchStateCheckSumComputeTactic
    {
        public MatchStateCheckSum GetMatchCheckSum(int currentEngineFrame, in MatchState matchState)
        {
            return new MatchStateCheckSum(currentEngineFrame,
                GetFieldCheckSum(matchState.Player1State),
                GetFieldCheckSum(matchState.Player2State));
        }

        protected abstract int GetFieldCheckSum(in PlayerState fieldState);
    }
}