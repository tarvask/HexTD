using Match.Field.Shooting.TargetFinding;

namespace Match.Field.Shooting
{
    public interface IShootable
    {
        bool IsAttackReady { get; }
        bool TryFindTarget(TargetFinder targetFinder, TargetContainer targetContainer);
        ProjectileController CreateAndInitProjectile(FieldFactory factory);
    }
}