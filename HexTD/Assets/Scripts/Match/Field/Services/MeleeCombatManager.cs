using Match.Field.Mob;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field.Services
{
    public class MeleeCombatManager : BaseDisposable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            
            public ReactiveCommand<MobController> AttackTowerByMobReactiveCommand { get; }

            public Context(FieldModel fieldModel,
                ReactiveCommand<MobController> attackTowerByMobReactiveCommand)
            {
                FieldModel = fieldModel;
                AttackTowerByMobReactiveCommand = attackTowerByMobReactiveCommand;
            }
        }
        
        private readonly Context _context;
        
        public MeleeCombatManager(Context context)
        {
            _context = context;

            AddDisposable(_context.AttackTowerByMobReactiveCommand.Subscribe(AttackTowerByMob));
        }

        private void AttackTowerByMob(MobController mob)
        {
            int damage = mob.Attack();
            TowerController attackedTower = _context.FieldModel.TowersManager.Towers[mob.BlockerId];
            attackedTower.Hurt(damage);
            Debug.Log($"Mob={mob.Id} hurt tower={mob.BlockerId} by {damage}, current health is {attackedTower.BaseReactiveModel.Health.Value.CurrentValue}");
        }
    }
}