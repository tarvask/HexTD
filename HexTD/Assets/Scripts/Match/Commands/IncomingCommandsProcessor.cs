using HexSystem;
using Match.Field.Tower;
using Match.State;
using Match.Wave;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Commands
{
    public class IncomingCommandsProcessor : BaseDisposable
    {
        public struct Context
        {
            public MatchCommands.IncomingCommands IncomingCommandsEnemy { get; }
            public MatchCommands.IncomingCommands IncomingCommandsOur { get; }
            public MatchCommonCommands.IncomingGeneralCommands IncomingCommandsCommon { get; }
            public IReadOnlyReactiveProperty<ProcessRoles> OurRoleReactiveProperty { get; }

            public Context(
                MatchCommands.IncomingCommands incomingCommandsEnemy,
                MatchCommands.IncomingCommands incomingCommandsOur,
                MatchCommonCommands.IncomingGeneralCommands incomingCommandsCommon,
                IReadOnlyReactiveProperty<ProcessRoles> ourRoleReactiveProperty)
            {
                IncomingCommandsEnemy = incomingCommandsEnemy;
                IncomingCommandsOur = incomingCommandsOur;
                IncomingCommandsCommon = incomingCommandsCommon;
                OurRoleReactiveProperty = ourRoleReactiveProperty;
            }
        }

        private readonly Context _context;

        public IncomingCommandsProcessor(Context context)
        {
            _context = context;
        }
        
        public void RequestSyncState(int timestamp)
        {
            _context.IncomingCommandsCommon.RequestSyncState.Fire();
        }

        public void ApplySyncState(MatchState matchState, int timestamp)
        {
            _context.IncomingCommandsCommon.ApplySyncState.Fire(matchState, timestamp);
        }

        public void BuildTower(ProcessRoles senderRole, Hex2d buildPosition, TowerShortParams towerShortParams, int timestamp)
        {
            if (senderRole == _context.OurRoleReactiveProperty.Value)
                _context.IncomingCommandsOur.ApplyBuildTower.Fire(buildPosition, towerShortParams);
            else
                _context.IncomingCommandsEnemy.ApplyBuildTower.Fire(buildPosition, towerShortParams);
        }
        
        public void UpgradeTower(ProcessRoles senderRole, Hex2d buildPosition, TowerShortParams towerShortParams, int timestamp)
        {
            if (senderRole == _context.OurRoleReactiveProperty.Value)
                _context.IncomingCommandsOur.ApplyUpgradeTower.Fire(buildPosition, towerShortParams);
            else
                _context.IncomingCommandsEnemy.ApplyUpgradeTower.Fire(buildPosition, towerShortParams);
        }

        public void SellTower(ProcessRoles senderRole, Hex2d buildPosition, TowerShortParams towerShortParams, int timestamp)
        {
            if (senderRole == _context.OurRoleReactiveProperty.Value)
                _context.IncomingCommandsOur.ApplySellTower.Fire(buildPosition, towerShortParams);
            else
                _context.IncomingCommandsEnemy.ApplySellTower.Fire(buildPosition, towerShortParams);
        }
        
        public void StartWaveSpawn(BuiltWaveParams waveParams, int randomSeed, int timestamp)
        {
            _context.IncomingCommandsCommon.StartWaveSpawn.Fire(waveParams, randomSeed);
        }
    }
}