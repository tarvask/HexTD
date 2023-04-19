using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
	[Serializable]
	public class GradualDamageTowerAttack : BaseAttackEffect
	{
		[SerializeField] private float damageCapacity;
		[SerializeField] private float damagePerDelay;
		[SerializeField] private float delay;

		public override void ApplyAttackImpact(ITargetable mobController, float sqrDistance)
		{
		}

		public override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
		{
			PoisonBuff poisonBuff = new PoisonBuff(damageCapacity, damagePerDelay, delay);
			buffManager.AddBuff(targetController, poisonBuff);
		}
	}
}