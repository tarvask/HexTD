using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
	[Serializable]
	public class SplashDamageTowerAttackEffect : BaseTowerAttackEffect
	{
		[SerializeField] private float percentOfBaseDamage;
		[SerializeField] private float radius;
		
		public override void ApplyAttack(IShootable mobController, BuffManager buffManager)
		{
			
		}
	}
}