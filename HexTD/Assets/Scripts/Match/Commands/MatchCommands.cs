using HexSystem;
using Match.Field.Tower;
using Tools;

namespace Match.Commands
{
    public class MatchCommands : BaseDisposable
    {
        public class OutgoingCommands : BaseDisposable
        {
            public ObservableEvent<Hex2d, TowerShortParams> RequestBuildTower { get; }
            public ObservableEvent<Hex2d, TowerShortParams> RequestUpgradeTower { get; }
            public ObservableEvent<Hex2d, TowerShortParams> RequestSellTower { get; }

            public OutgoingCommands()
            {
                RequestBuildTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
                RequestUpgradeTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
                RequestSellTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
            }
        }
        
        public class IncomingCommands : BaseDisposable
        {
            public ObservableEvent<Hex2d, TowerShortParams> ApplyBuildTower { get; }
            public ObservableEvent<Hex2d, TowerShortParams> ApplyUpgradeTower { get; }
            public ObservableEvent<Hex2d, TowerShortParams> ApplySellTower { get; }

            public IncomingCommands()
            {
                ApplyBuildTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
                ApplyUpgradeTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
                ApplySellTower = AddDisposable(new ObservableEvent<Hex2d, TowerShortParams>());
            }
        }

        public OutgoingCommands Outgoing { get; }
        public IncomingCommands Incoming { get; }

        public MatchCommands(OutgoingCommands outgoingCommands, IncomingCommands incomingCommands)
        {
            Outgoing = AddDisposable(outgoingCommands);
            Incoming = AddDisposable(incomingCommands);
        }
    }
}