using System;

namespace Match.State.CheckSum
{
    public readonly struct MatchStateCheckSum
    {
        public readonly int EngineFrame;
        public readonly int Player1CheckSum;
        public readonly int Player2CheckSum;

        public MatchStateCheckSum(int engineFrame, int player1CheckSum, int player2CheckSum)
        {
            EngineFrame = engineFrame;
            Player1CheckSum = player1CheckSum;
            Player2CheckSum = player2CheckSum;
        }
    }
}