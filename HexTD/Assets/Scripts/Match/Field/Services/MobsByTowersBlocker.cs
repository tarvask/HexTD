using System;
using System.Collections.Generic;
using Match.Field.Mob;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field.Services
{
    public class MobsByTowersBlocker : BaseDisposable
    {
        public struct Context
        {
            public Vector3 SingleHexSize { get; }
            public TowersManager TowersManager { get; }
            
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }

            public Context(Vector3 singleHexSize, TowersManager towersManager,
                ReactiveCommand<MobController> removeMobReactiveCommand)
            {
                SingleHexSize = singleHexSize;
                TowersManager = towersManager;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
            }
        }
        
        private readonly Context _context;
        private readonly float _singleHexSizeSqr;
        
        private readonly Dictionary<int, List<MobController>> _mobsByTowers;

        public MobsByTowersBlocker(Context context)
        {
            _context = context;

            _singleHexSizeSqr = _context.SingleHexSize.x * _context.SingleHexSize.x;
            _mobsByTowers = new Dictionary<int, List<MobController>>(TestMatchEngine.TowersInHandCount);

            AddDisposable(_context.RemoveMobReactiveCommand.Subscribe(RemoveMobFromBlockerTower));
            AddDisposable(_context.TowersManager.TowerBuiltReactiveCommand.Subscribe(AddTower));
            AddDisposable(_context.TowersManager.TowerRemovedReactiveCommand.Subscribe(RemoveTower));
        }

        public bool TryGetBlockingTowerForMob(MobController mob, out int possibleBlockerId)
        {
            possibleBlockerId = -1;
            int positionHash = mob.CurrentTargetHexPosition.GetHashCode();

            if (!_context.TowersManager.TowerContainer.TryGetTowerInPositionHash(positionHash,
                    out TowerController possibleBlocker))
                return false;
            
            if ((mob.Position - possibleBlocker.Position).sqrMagnitude > _singleHexSizeSqr)
                return false;

            if (!_mobsByTowers.TryGetValue(possibleBlocker.Id, out var blockedMobs))
                throw new ArgumentException($"Tried to block mob tower with id={possibleBlocker.Id} that is not in registry");

            if (blockedMobs.Count < possibleBlocker.MaxEnemyBlocked)
            {
                blockedMobs.Add(mob);
                possibleBlockerId = possibleBlocker.Id;
                return true;
            }

            return false;
        }

        private void RemoveMobFromBlockerTower(MobController mob)
        {
            if (mob.BlockerId == 0)
                return;

            if (!_mobsByTowers.TryGetValue(mob.BlockerId, out var mobsList))
                throw new ArgumentException(
                    $"Tried to remove attacking mob from non-registered tower with id={mob.BlockerId}");

            mobsList.Remove(mob);
        }

        private void AddTower(TowerController tower)
        {
            _mobsByTowers.Add(tower.Id, new List<MobController>(tower.MaxEnemyBlocked));
        }

        private void RemoveTower(TowerController tower)
        {
            if (!_mobsByTowers.TryGetValue(tower.Id, out var mobsList))
                throw new ArgumentException(
                    $"Tried to remove non-registered tower with id={tower.Id}");

            foreach (MobController blockedMob in mobsList)
                blockedMob.Unblock();
            
            mobsList.Clear();
            _mobsByTowers.Remove(tower.Id);
        }

        public void Clear()
        {
            foreach (KeyValuePair<int,List<MobController>> towerWithMobs in _mobsByTowers)
                towerWithMobs.Value.Clear();
            
            _mobsByTowers.Clear();
        }

        protected override void OnDispose()
        {
            Clear();
            
            base.OnDispose();
        }
    }
}