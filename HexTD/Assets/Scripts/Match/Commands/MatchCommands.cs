using Match.Field.Tower;
using Tools;
using UnityEngine;

namespace Match.Commands
{
    public class MatchCommands : BaseDisposable
    {
        public class OutgoingCommands : BaseDisposable
        {
            public ObservableEvent<Vector2Int, TowerShortParams> RequestBuildTower { get; }
            public ObservableEvent<Vector2Int, TowerShortParams> RequestUpgradeTower { get; }
            public ObservableEvent<Vector2Int, TowerShortParams> RequestSellTower { get; }

            public OutgoingCommands()
            {
                RequestBuildTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
                RequestUpgradeTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
                RequestSellTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
            }
        }
        
        public class IncomingCommands : BaseDisposable
        {
            public ObservableEvent<Vector2Int, TowerShortParams> ApplyBuildTower { get; }
            public ObservableEvent<Vector2Int, TowerShortParams> ApplyUpgradeTower { get; }
            public ObservableEvent<Vector2Int, TowerShortParams> ApplySellTower { get; }

            public IncomingCommands()
            {
                ApplyBuildTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
                ApplyUpgradeTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
                ApplySellTower = AddDisposable(new ObservableEvent<Vector2Int, TowerShortParams>());
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