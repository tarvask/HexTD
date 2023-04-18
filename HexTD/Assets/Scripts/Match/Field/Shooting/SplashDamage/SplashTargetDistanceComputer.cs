using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.Shooting.SplashDamage
{
    public static class SplashTargetDistanceComputer
    {
        public static void GetTargetsWithSqrDistances(ProjectileController projectile,
            IEnumerable<ITargetable> shootables, ref List<TargetWithSqrDistancePair> targetWithDistances)
        {
            Vector3 projectilePosition = projectile.CurrentPosition;
            float projectileSplashSqrRadius = projectile.SplashDamageRadius * projectile.SplashDamageRadius;
            
            foreach (var target in shootables)
            {
                float shootableToProjectileSqrDistance =
                    (target.Position - projectilePosition).sqrMagnitude;
                
                if (shootableToProjectileSqrDistance <= projectileSplashSqrRadius)
                    targetWithDistances.Add(new TargetWithSqrDistancePair(target,
                        shootableToProjectileSqrDistance));
            }
        }
    }
}