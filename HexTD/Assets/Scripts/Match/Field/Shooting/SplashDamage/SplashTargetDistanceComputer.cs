using System.Collections.Generic;
using HexSystem;
using Match.Field.AttackEffect;
using Match.Field.Hexagons;

namespace Match.Field.Shooting.SplashDamage
{
    public static class SplashTargetDistanceComputer
    {
        public static void GetTargetsWithSqrDistances(ProjectileController projectile,
            IHexPositionConversionService hexPositionConversionService,
            IReadOnlyDictionary<int, List<ITarget>> targetsByPosition, 
            List<TargetWithSqrDistancePair> targetWithDistances)
        {
            BaseSplashAttack splashAttack = projectile.BaseAttackEffect as BaseSplashAttack;
            if(splashAttack == null)
                return;

            int radius = splashAttack.SplashRadiusInHex;
            Hex2d projectilePosition = hexPositionConversionService.ToHexFromWorldPosition(projectile.CurrentPosition, false);

            foreach (var hex in Hex2d.IterateSpiralRing(projectilePosition, radius))
            {
                int hexHash = hex.GetHashCode();
                if (!targetsByPosition.TryGetValue(hexHash, out var targets))
                    continue;

                foreach (var target in targets)
                {
                    float sqrDistance = (target.Position - projectile.CurrentPosition).sqrMagnitude;
                    targetWithDistances.Add(new TargetWithSqrDistancePair(target, sqrDistance));
                }
            }
        }
    }
}