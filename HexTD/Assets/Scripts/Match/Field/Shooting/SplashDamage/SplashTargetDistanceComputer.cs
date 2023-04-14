using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.Shooting.SplashDamage
{
    public static class SplashTargetDistanceComputer
    {
        public static void GetTargetsWithSqrDistances(ProjectileController projectile,
            Dictionary<int, ITargetable> shootables, ref List<TargetWithSqrDistancePair> targetWithDistances)
        {
            Vector3 projectilePosition = projectile.CurrentPosition;
            float projectileSplashSqrRadius = projectile.SplashDamageRadius * projectile.SplashDamageRadius;
            
            foreach (KeyValuePair<int,ITargetable> shootablePair in shootables)
            {
                float shootableToProjectileSqrDistance =
                    (shootablePair.Value.Position - projectilePosition).sqrMagnitude;
                
                if (shootableToProjectileSqrDistance <= projectileSplashSqrRadius)
                    targetWithDistances.Add(new TargetWithSqrDistancePair(shootablePair.Key,
                        shootableToProjectileSqrDistance));
            }
        }
    }
}