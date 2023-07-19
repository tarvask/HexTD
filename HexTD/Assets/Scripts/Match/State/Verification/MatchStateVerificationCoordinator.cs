using System;
using Match.Commands;
using Match.State.CheckSum;
using Tools;
using UniRx;

namespace Match.State.Verification
{
    public class MatchStateVerificationCoordinator : BaseDisposable
    {
        public struct Context
        {
            public MatchStateCheckSumComputerController MatchStateCheckSumComputerController { get; }
            public IReadOnlyReactiveProperty<NetworkRoles> OurNetworkRoleReactiveProperty { get; }
            public ReactiveCommand<MatchStateCheckSum> MatchStateCheckSumComputedReactiveCommand { get; }
            public ReactiveCommand RequestMatchStateReactiveCommand { get; }
            public MatchCommonCommands.IncomingGeneralCommands IncomingGeneralCommands { get; }
            public MatchCommonCommands.ServerCommands ServerCommands { get; }

            public Context(
                MatchStateCheckSumComputerController matchStateCheckSumComputerController,
                IReadOnlyReactiveProperty<NetworkRoles> ourNetworkRoleReactiveProperty,
                ReactiveCommand<MatchStateCheckSum> matchStateCheckSumComputedReactiveCommand,
                ReactiveCommand requestMatchStateReactiveCommand,
                MatchCommonCommands.IncomingGeneralCommands incomingGeneralCommands,
                MatchCommonCommands.ServerCommands serverCommands)
            {
                MatchStateCheckSumComputerController = matchStateCheckSumComputerController;
                OurNetworkRoleReactiveProperty = ourNetworkRoleReactiveProperty;
                MatchStateCheckSumComputedReactiveCommand = matchStateCheckSumComputedReactiveCommand;
                RequestMatchStateReactiveCommand = requestMatchStateReactiveCommand;
                IncomingGeneralCommands = incomingGeneralCommands;
                ServerCommands = serverCommands;
            }
        }

        private readonly Context _context;
        private readonly MatchStateVerificationBase _serverImplementation;
        private readonly MatchStateVerificationBase _clientImplementation;
        private MatchStateVerificationBase _currentImplementation;

        public MatchStateVerificationCoordinator(Context context)
        {
            _context = context;

            MatchStateVerificationBase.Context stateVerificationImplementationContext =
                new MatchStateVerificationBase.Context(_context.MatchStateCheckSumComputerController,
                    _context.RequestMatchStateReactiveCommand,
                    _context.ServerCommands);
            _serverImplementation = new MatchStateVerificationServer(stateVerificationImplementationContext);
            _clientImplementation = new MatchStateVerificationClient(stateVerificationImplementationContext);

            _context.OurNetworkRoleReactiveProperty.Subscribe(UpdateRole);
            _context.MatchStateCheckSumComputedReactiveCommand.Subscribe(_currentImplementation.BroadcastMatchCheckSum);
            _context.IncomingGeneralCommands.BroadcastStateCheckSum.Subscribe(_currentImplementation.VerifyCheckSum);
        }
        
        private void UpdateRole(NetworkRoles newRole)
        {
            switch (newRole)
            {
                case NetworkRoles.Client:
                    _currentImplementation = _clientImplementation;
                    break;
                case NetworkRoles.Server:
                    _currentImplementation = _serverImplementation;
                    break;
                default:
                    throw new ArgumentException("Bad network role");
            }
        }
    }
}