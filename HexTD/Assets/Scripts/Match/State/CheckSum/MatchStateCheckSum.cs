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

        public bool Equals(MatchStateCheckSum other)
        {
            return EngineFrame == other.EngineFrame && Player1CheckSum == other.Player1CheckSum && Player2CheckSum == other.Player2CheckSum;
        }

        public override bool Equals(object other)
        {
            return other is MatchStateCheckSum checkSum && Equals(checkSum);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EngineFrame, Player1CheckSum, Player2CheckSum);
        }
        
        public static bool operator == (MatchStateCheckSum checkSum1, MatchStateCheckSum checkSum2)
        {
            return checkSum1.Equals(checkSum2);
        }
        
        public static bool operator != (MatchStateCheckSum checkSum1, MatchStateCheckSum checkSum2)
        {
            return !checkSum1.Equals(checkSum2);
        }
    }
}