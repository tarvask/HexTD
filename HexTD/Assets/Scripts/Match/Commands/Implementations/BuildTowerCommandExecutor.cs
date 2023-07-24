using System.Threading.Tasks;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Commands.Implementations
{
    public class BuildTowerCommandExecutor : AbstractCommandExecutor
    {
        public BuildTowerCommandExecutor(Context context) : base(context) { }

        protected override ICommandImplementation CreateServerImplementation(Context context)
        {
            return new ServerImplementation(context);
        }

        protected override ICommandImplementation CreateClientImplementation(Context context)
        {
            return new ClientImplementation(context);
        }

        private abstract class BaseBuildTowerCommandImplementation : AbstractCommandImplementation
        {
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

            protected BaseBuildTowerCommandImplementation(Context context) : base(context) { }

            protected Parameters ExtractParameters(Hashtable commandParametersTable)
            {
                Hex2d towerPosition = new Hex2d 
                ((byte)commandParametersTable[PhotonEventsConstants.BuildTower.PositionQParam],
                    (byte)commandParametersTable[PhotonEventsConstants.BuildTower.PositionRParam]);
                TowerShortParams towerConfig = new TowerShortParams((TowerType)(byte)commandParametersTable[PhotonEventsConstants.BuildTower.TowerTypeParam],
                    (int)commandParametersTable[PhotonEventsConstants.BuildTower.TowerLevelParam]);
                int timeStamp = (int)commandParametersTable[PhotonEventsConstants.BuildTower.TimeParam];
                ProcessRoles senderRole = (ProcessRoles)(byte)commandParametersTable[PhotonEventsConstants.BuildTower.RoleParam];//_networkMatchStatus.UserIdsAndRoles[senderId];
                
                return new Parameters(senderRole, towerPosition, towerConfig, timeStamp);
            }
        }

        private class ServerImplementation : BaseBuildTowerCommandImplementation
        {
            public ServerImplementation(Context context) : base(context) { }
            
            public override async Task Request(Hashtable commandParametersTable)
            {
                Parameters commandParameters = ExtractParameters(commandParametersTable);
                
                // compensate ping
                int targetExecutionTimeStamp = Context.CurrentEngineFrameReactiveProperty.Value + Context.PingDamperFramesDeltaReactiveProperty.Value;
                // modify timestamp parameter
                commandParametersTable[PhotonEventsConstants.BuildTower.TimeParam] = targetExecutionTimeStamp;
                // send further
                Context.EventBus.RaiseEvent(PhotonEventsConstants.BuildTower.ApplyEventId, commandParametersTable);
                
                Debug.Log($"Requesting building tower, current frame = {Context.CurrentEngineFrameReactiveProperty.Value}, " +
                          $"target frame = {targetExecutionTimeStamp}");
                // wait for target frame
                await WaitForTargetFrame(targetExecutionTimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.BuildTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig,
                    targetExecutionTimeStamp);
                
                await Task.CompletedTask;
            }

            public override async Task Apply(Hashtable commandParametersTable)
            {
                // do nothing
                await Task.CompletedTask;
            }
        }
        
        private class ClientImplementation : BaseBuildTowerCommandImplementation
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
                
                Debug.Log($"Applying tower build, current frame = {Context.CurrentEngineFrameReactiveProperty.Value}, " +
                          $"target frame = {commandParameters.TimeStamp}");
                // wait for target frame
                await WaitForTargetFrame(commandParameters.TimeStamp);
                // handle
                Context.MatchEngine.IncomingCommandsProcessor.BuildTower(commandParameters.SenderRole,
                    commandParameters.TowerPosition, commandParameters.TowerConfig,
                    commandParameters.TimeStamp);
                
                await Task.CompletedTask;
            }
        }
    }
}