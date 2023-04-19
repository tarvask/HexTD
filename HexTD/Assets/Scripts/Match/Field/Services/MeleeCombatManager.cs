using Match.Field.Mob;
using Tools;
using UniRx;

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
            _context.FieldModel.TowersManager.Towers[mob.BlockerId].Hurt(mob.Attack());
        }
    }
}