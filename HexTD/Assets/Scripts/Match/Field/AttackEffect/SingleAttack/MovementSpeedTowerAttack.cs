using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
	[Serializable]
	public class MovementSpeedTowerAttack : BaseAttackEffect
	{
		[SerializeField] private float duration;
		[SerializeField] private float percentageValue;

		public void ApplyAttackImpact(ITarget attackerController, float sqrDistance)
		{
		}

		public void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager)
		{
			DivideFloatValueBuff speedBuff = new DivideFloatValueBuff(percentageValue);
			speedBuff.AddCondition(new TimerBuffCondition(duration));
			
			if(attackerController.BaseReactiveModel.TryGetBuffableValue(EntityBuffableValueType.Damage,
				   out var buffableValue))
				buffManager.AddBuff(buffableValue, speedBuff);
		}
	}
}