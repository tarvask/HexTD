using ExitGames.Client.Photon;
using Match.EventBus;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Commands
{
    public class OutgoingCommandsProcessor : BaseDisposable
    {
        public struct Context
        {
            public TestMatchEngine TestMatchEngine { get; }
            public IEventBus EventBus { get; }
            public IReadOnlyReactiveProperty<ProcessRoles> OurRoleReactiveProperty { get; }
            public MatchCommands.OutgoingCommands OutgoingCommands { get; }

            public Context(TestMatchEngine testMatchEngine, IEventBus eventBus, IReadOnlyReactiveProperty<ProcessRoles> ourRoleReactiveProperty,
                MatchCommands.OutgoingCommands outgoingCommands)
            {
                TestMatchEngine = testMatchEngine;
                EventBus = eventBus;
                OurRoleReactiveProperty = ourRoleReactiveProperty;
                OutgoingCommands = outgoingCommands;
            }
        }

        private readonly Context _context;

        public OutgoingCommandsProcessor(Context context)
        {
            _context = context;
            
            _context.OutgoingCommands.RequestBuildTower.Subscribe(RequestBuildTower);
            _context.OutgoingCommands.RequestUpgradeTower.Subscribe(RequestUpgradeTower);
            _context.OutgoingCommands.RequestSellTower.Subscribe(RequestSellTower);
        }

        private void RequestBuildTower(Vector2Int buildPosition, TowerShortParams towerShortParams)
        {
            Hashtable requestBuildTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.BuildTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.BuildTower.PositionXParam, (byte)buildPosition.x},
                {PhotonEventsConstants.BuildTower.PositionYParam, (byte)buildPosition.y},
                {PhotonEventsConstants.BuildTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.BuildTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.BuildTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.BuildTower.RequestEventId, requestBuildTowerProperties);
        }
        
        private void RequestUpgradeTower(Vector2Int upgradePosition, TowerShortParams towerShortParams)
        {
            Hashtable requestUpgradeTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.UpgradeTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.UpgradeTower.PositionXParam, (byte)upgradePosition.x},
                {PhotonEventsConstants.UpgradeTower.PositionYParam, (byte)upgradePosition.y},
                {PhotonEventsConstants.UpgradeTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.UpgradeTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.UpgradeTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.UpgradeTower.RequestEventId, requestUpgradeTowerProperties);
        }

        private void RequestSellTower(Vector2Int sellPosition, TowerShortParams towerShortParams)
        {
            Hashtable requestUpgradeTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.SellTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.SellTower.PositionXParam, (byte)sellPosition.x},
                {PhotonEventsConstants.SellTower.PositionYParam, (byte)sellPosition.y},
                {PhotonEventsConstants.SellTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.SellTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.SellTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.SellTower.RequestEventId, requestUpgradeTowerProperties);
        }
    }
}