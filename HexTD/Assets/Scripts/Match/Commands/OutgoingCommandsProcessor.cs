using ExitGames.Client.Photon;
using HexSystem;
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

        private void RequestBuildTower(Hex2d buildPosition, TowerShortParams towerShortParams)
        {
            Debug.Log($"Requesting tower build, current frame = {_context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}");
            Hashtable requestBuildTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.BuildTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.BuildTower.PositionQParam, (byte)buildPosition.Q},
                {PhotonEventsConstants.BuildTower.PositionRParam, (byte)buildPosition.R},
                {PhotonEventsConstants.BuildTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.BuildTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.BuildTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.BuildTower.RequestEventId, requestBuildTowerProperties);
        }
        
        private void RequestUpgradeTower(Hex2d upgradePosition, TowerShortParams towerShortParams)
        {
            Hashtable requestUpgradeTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.UpgradeTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.BuildTower.PositionQParam, (byte)upgradePosition.Q},
                {PhotonEventsConstants.BuildTower.PositionRParam, (byte)upgradePosition.R},
                {PhotonEventsConstants.UpgradeTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.UpgradeTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.UpgradeTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.UpgradeTower.RequestEventId, requestUpgradeTowerProperties);
        }

        private void RequestSellTower(Hex2d sellPosition, TowerShortParams towerShortParams)
        {
            Hashtable requestUpgradeTowerProperties = new Hashtable
            {
                {PhotonEventsConstants.SellTower.RoleParam, (byte)_context.OurRoleReactiveProperty.Value},
                {PhotonEventsConstants.BuildTower.PositionQParam, (byte)sellPosition.Q},
                {PhotonEventsConstants.BuildTower.PositionRParam, (byte)sellPosition.R},
                {PhotonEventsConstants.SellTower.TowerTypeParam, (byte)towerShortParams.TowerType},
                {PhotonEventsConstants.SellTower.TowerLevelParam, towerShortParams.Level},
                {PhotonEventsConstants.SellTower.TimeParam, _context.TestMatchEngine.CurrentEngineFrameReactiveProperty.Value}
            };
            _context.EventBus.RaiseEvent(PhotonEventsConstants.SellTower.RequestEventId, requestUpgradeTowerProperties);
        }
    }
}