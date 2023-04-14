using System;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	[Serializable]
	public class GradualDamageTowerAttackEffect : BaseTowerAttackEffect
	{
		[SerializeField] private float damageCapacity;
		[SerializeField] private float damagePerDelay;
		[SerializeField] private float delay;

		public override void ApplyAttack(IShootable mobController, BuffManager buffManager)
		{
			PoisonBuff poisonBuff = new PoisonBuff(damageCapacity, damagePerDelay, delay);
			buffManager.AddBuff(mobController, poisonBuff);
		}
	}
}