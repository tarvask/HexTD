using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Commands.Implementations
{
    public class UpgradeTowerCommandExecutor : AbstractCommandExecutor
    {
        public UpgradeTowerCommandExecutor(Context context) : base(context) { }

        protected override ICommandImplementation CreateServerImplementation(Context context)
        {
            return new ServerImplementation(context);
        }

        protected override ICommandImplementation CreateClientImplementation(Context context)
        {
            return new ClientImplementation(context);
        }
        
        private abstract class BaseUpgradeTowerCommandImplementation : AbstractCommandImplementation
        {
            protected struct Parameters
            {
                public ProcessRoles SenderRole { get; }
                public Vector2Int TowerPosition { get; }
                public TowerShortParams TowerConfig { get; }
                public int TimeStamp { get; }

                public Parameters(ProcessRoles senderRole, Vector2Int towerPosition,
                    TowerShortParams towerConfig, int timeStamp)
                {
                    SenderRole = senderRole;
                    TowerPosition = towerPosition;
                    TowerConfig = towerConfig;
                    TimeStamp = timeStamp;
                }
            }

            protected BaseUpgradeTowerCommandImplementation(Context context) : base(context) { }

            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                Vector2Int towerPosition = new Vector2Int
                ((byte)commandParametersTable[PhotonEventsConstants.UpgradeTower.PositionXParam],
                    (byte)commandParametersTable[PhotonEventsConstants.UpgradeTower.PositionYParam]);
                TowerShortParams towerConfig = new TowerShortParams((TowerType)(byte)commandParametersTable[PhotonEventsConstants.UpgradeTower.TowerTypeParam],
                    (int)commandParametersTable[PhotonEventsConstants.UpgradeTower.TowerLevelParam]);
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.UpgradeTower.TimeParam];
                ProcessRoles senderRole = (ProcessRoles)(byte)commandParametersTable[PhotonEventsConstants.UpgradeTower.RoleParam];
                
                return new Parameters(senderRole, towerPosition, towerConfig, timeStamp);
            }
        }

        private class ServerImplementation : BaseUpgradeTowerCommandImplementation
        {
            public ServerImplementation(Context context) : base(context) { }

            public override async Task Request(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // compensate ping
                int targetExecutionTimeStamp = Context.CurrentEngineFrameReactiveProperty.Value + Context.PingDamperFramesDeltaReactiveProperty.Value;
                // modify timestamp parameter
                commandParametersTable[PhotonEventsConstants.UpgradeTower.TimeParam] = targetExecutionTimeStamp;
                // send further
                Context.EventBus.RaiseEvent(PhotonEventsConstants.UpgradeTower.ApplyEventId, commandParametersTable);
                
                // wait for target frame
                await WaitForTargetFrame(targetExecutionTimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.UpgradeTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig, commandParameters.TimeStamp);

                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }
        }

        private class ClientImplementation : BaseUpgradeTowerCommandImplementation
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
                Context.MatchEngine.IncomingCommandsProcessor.UpgradeTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig, commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}