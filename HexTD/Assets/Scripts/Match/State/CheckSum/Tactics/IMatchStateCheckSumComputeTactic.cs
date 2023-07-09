namespace Match.State.CheckSum
{
    public interface IMatchStateCheckSumComputeTactic
    {
        MatchStateCheckSum GetMatchCheckSum(int currentEngineFrame, in MatchState matchState);
    }
}