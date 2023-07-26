using Match.Commands;
using Match.State.CheckSum;
using Tools;
using UniRx;

namespace Match.State.Verification
{
    public abstract class MatchStateVerificationBase : BaseDisposable
    {
        public struct Context
        {
            public MatchStateCheckSumComputerController MatchStateCheckSumComputerController { get; }
            public ReactiveCommand RequestMatchStateReactiveCommand { get; }
            public MatchCommonCommands.ServerCommands ServerCommands { get; }

            public Context(
                MatchStateCheckSumComputerController matchStateCheckSumComputerController,
                ReactiveCommand requestMatchStateReactiveCommand,
                MatchCommonCommands.ServerCommands serverCommands)
            {
                MatchStateCheckSumComputerController = matchStateCheckSumComputerController;
                RequestMatchStateReactiveCommand = requestMatchStateReactiveCommand;
                ServerCommands = serverCommands;
            }
        }

        protected readonly Context _context;

        protected MatchStateVerificationBase(Context context)
        {
            _context = context;
        }

        public abstract void BroadcastMatchCheckSum(MatchStateCheckSum checkSum);

        public abstract void VerifyCheckSum(MatchStateCheckSum checkSum);
    }
}