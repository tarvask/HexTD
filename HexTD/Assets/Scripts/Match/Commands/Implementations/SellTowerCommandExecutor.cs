using System.Threading.Tasks;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field.Tower;

namespace Match.Commands.Implementations
{
    public class SellTowerCommandExecutor : AbstractCommandExecutor
    {
        public SellTowerCommandExecutor(Context context) : base(context) { }

        protected override ICommandImplementation CreateServerImplementation(Context context)
        {
            return new ServerImplementation(context);
        }

        protected override ICommandImplementation CreateClientImplementation(Context context)
        {
            return new ClientImplementation(context);
        }

        private abstract class BaseSellTowerCommandImplementation : AbstractCommandImplementation
        {
            protected BaseSellTowerCommandImplementation(Context context) : base(context) { }

            protected struct Parameters
            {
                public ProcessRoles SenderRole { get; }
                public Hex2d TowerPosition { get; }
                public TowerShortParams TowerConfig { get; }
                public int TimeStamp { get; }

                public Parameters(ProcessRoles senderRole, Hex2d towerPosition,
                    TowerShortParams towerConfig, int timeStamp)
                {
                    SenderRole = senderRole;
                    TowerPosition = towerPosition;
                    TowerConfig = towerConfig;
                    TimeStamp = timeStamp;
                }
            }
            
            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                Hex2d towerPosition = new Hex2d
                ((byte)commandParametersTable[PhotonEventsConstants.SellTower.PositionQParam],
                    (byte)commandParametersTable[PhotonEventsConstants.SellTower.PositionRParam]);
                TowerShortParams towerConfig = new TowerShortParams((TowerType)(byte)commandParametersTable[PhotonEventsConstants.SellTower.TowerTypeParam],
                    (int)commandParametersTable[PhotonEventsConstants.SellTower.TowerLevelParam]);
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.SellTower.TimeParam];
                ProcessRoles senderRole = (ProcessRoles)(byte)commandParametersTable[PhotonEventsConstants.SellTower.RoleParam];
                
                return new Parameters(senderRole, towerPosition, towerConfig, timeStamp);
            }
        }

        private class ServerImplementation : BaseSellTowerCommandImplementation
        {
            public ServerImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // compensate ping
                int targetExecutionTimeStamp = Context.CurrentEngineFrameReactiveProperty.Value + Context.PingDamperFramesDeltaReactiveProperty.Value;
                // modify timestamp parameter
                commandParametersTable[PhotonEventsConstants.SellTower.TimeParam] = targetExecutionTimeStamp;
                // send further
                Context.EventBus.RaiseEvent(PhotonEventsConstants.SellTower.ApplyEventId, commandParametersTable);
                
                // wait for target frame
                await WaitForTargetFrame(targetExecutionTimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.SellTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig, commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }
        }

        private class ClientImplementation : BaseSellTowerCommandImplementation
        {
            public ClientImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // wait for target frame
                await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.SellTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig, commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}