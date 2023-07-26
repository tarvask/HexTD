using Match.State.CheckSum;

namespace Match.State.Verification
{
    public class MatchStateVerificationServer : MatchStateVerificationBase
    {
        public MatchStateVerificationServer(Context context) : base(context)
        {
        }
        
        public override void BroadcastMatchCheckSum(MatchStateCheckSum checkSum)
        {
            _context.ServerCommands.BroadcastStateCheckSum.Fire(checkSum);
        }

        public override void VerifyCheckSum(MatchStateCheckSum checkSum)
        {
            // do nothing
        }
    }
}