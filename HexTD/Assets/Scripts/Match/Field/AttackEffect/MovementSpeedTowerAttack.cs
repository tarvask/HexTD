﻿using System;
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

		protected override void ApplyAttackImpact(ITargetable mobController, BuffManager buffManager)
		{
		}

		protected override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
		{
			DivideFloatValueBuff speedBuff = new DivideFloatValueBuff(percentageValue);
			speedBuff.AddCondition(new TimerBuffCondition(duration));
			
			if(targetController.BaseReactiveModel.TryGetBuffableValue(EntityBuffableValueType.Damage,
				   out var buffableValue))
				buffManager.AddBuff(buffableValue, speedBuff);
		}
	}
}