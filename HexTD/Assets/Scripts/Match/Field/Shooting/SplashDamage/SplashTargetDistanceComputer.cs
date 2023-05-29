using System;
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
            in IReadOnlyDictionary<int, List<ITarget>> targetsByPosition, 
            List<TargetWithSqrDistancePair> targetWithDistances)
        {
            Hex2d projectilePosition =
                hexPositionConversionService.ToHexFromWorldPosition(projectile.CurrentPosition, false);
            
            if (projectile.BaseAttackEffect is BaseSplashAttack splashAttack)
            {
                int auraRadiusInHex = splashAttack.SplashRadiusInHex;
                GetTargetsAroundPosition(projectilePosition, auraRadiusInHex, projectile, targetsByPosition,
                    targetWithDistances,
                    -1, (f1, f2) => true);
            }
            else if (projectile.BaseAttackEffect is BaseSingleAttack volumeAttack)
            {
                float volumeRadiusInUnits = volumeAttack.SplashRadiusInUnits;
                int volumeRadiusInHexesUpper = (int)(volumeRadiusInUnits / hexPositionConversionService.HexSize.x) + 1;
                GetTargetsAroundPosition(projectilePosition, volumeRadiusInHexesUpper, projectile, targetsByPosition,
                    targetWithDistances,
                    volumeRadiusInUnits * volumeRadiusInUnits,
                    (f1, f2) => f1 <= f2);
            }
        }

        private static void GetTargetsAroundPosition(in Hex2d projectilePosition, in int radius,
            ProjectileController projectile,
            in IReadOnlyDictionary<int, List<ITarget>> targetsByPosition,
            List<TargetWithSqrDistancePair> targetWithDistances,
            float sqrDistanceForCondition,
            Func<float, float, bool> sqrDistanceCondition)
        {
            foreach (var hex in Hex2d.IterateSpiralRing(projectilePosition, radius))
            {
                int hexHash = hex.GetHashCode();
                if (!targetsByPosition.TryGetValue(hexHash, out var targets))
                    continue;

                foreach (var target in targets)
                {
                    float sqrDistance = (target.Position - projectile.CurrentPosition).sqrMagnitude;
                    
                    if (sqrDistanceCondition(sqrDistance, sqrDistanceForCondition))
                        targetWithDistances.Add(new TargetWithSqrDistancePair(target, sqrDistance));
                }
            }
        }
    }
}