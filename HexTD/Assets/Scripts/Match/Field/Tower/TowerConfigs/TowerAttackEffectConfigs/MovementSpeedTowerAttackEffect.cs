using System;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	[Serializable]
	public class MovementSpeedTowerAttackEffect : BaseTowerAttackEffect
	{
		[SerializeField] private float duration;
		[SerializeField] private float percentageValue;
		
		public override void ApplyAttack(IShootable mobController, BuffManager buffManager)
		{
			DivideFloatValueBuff speedBuff = new DivideFloatValueBuff(percentageValue);
			speedBuff.AddCondition(new TimerBuffCondition(duration));
			buffManager.AddBuff(mobController.Speed, speedBuff);
		}
	}
}