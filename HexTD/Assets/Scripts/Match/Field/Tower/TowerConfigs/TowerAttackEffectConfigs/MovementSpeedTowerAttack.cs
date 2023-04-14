using System;
using System.Collections.Generic;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	[Serializable]
	public class MovementSpeedTowerAttack : BaseTowerAttack
	{
		[SerializeField] private float duration;
		[SerializeField] private float percentageValue;

		public override void ApplyAttackImpact(IShootable mobController, IEnumerable<IBuff<float>> damageBuffs)
		{
		}

		public override void ApplyAttackEffect(IShootable mobController, BuffManager buffManager)
		{
			DivideFloatValueBuff speedBuff = new DivideFloatValueBuff(percentageValue);
			speedBuff.AddCondition(new TimerBuffCondition(duration));
			buffManager.AddBuff(mobController.Speed, speedBuff);
		}
	}
}