using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	public class SplashDamageTowerAttackEffectConfig : ITowerAttackEffectConfig
	{
		[SerializeField] private float percentOfBaseDamage;
		[SerializeField] private float radius;
	}
}