using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	public class GradualDamageTowerAttackEffectConfig : ITowerAttackEffectConfig
	{
		[SerializeField] private float percentOfBaseDamage;
		[SerializeField] private float duration;
	}
}