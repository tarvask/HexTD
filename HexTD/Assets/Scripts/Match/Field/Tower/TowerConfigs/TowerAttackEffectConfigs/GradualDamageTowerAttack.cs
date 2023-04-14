using System;
using System.Collections.Generic;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	[Serializable]
	public class GradualDamageTowerAttack : BaseTowerAttack
	{
		[SerializeField] private float damageCapacity;
		[SerializeField] private float damagePerDelay;
		[SerializeField] private float delay;

		public override void ApplyAttackImpact(IShootable mobController, IEnumerable<IBuff<float>> damageBuffs)
		{
		}

		public override void ApplyAttackEffect(IShootable mobController, BuffManager buffManager)
		{
			PoisonBuff poisonBuff = new PoisonBuff(damageCapacity, damagePerDelay, delay);
			buffManager.AddBuff(mobController, poisonBuff);
		}
	}
}