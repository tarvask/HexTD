using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
	[Serializable]
	public class TowerAttackConfig
	{
		[SerializeField] private float baseDamage;
		[SerializeField] private float cooldown;
		[SerializeField] private int attackRadiusInHex;
		[SerializeReference] [InlineEditor] private List<BaseTowerAttackEffect> towerAttackEffectConfigs;

		public float BaseDamage => baseDamage;
		public float Cooldown => cooldown;
		public int AttackRadiusInHex => attackRadiusInHex;
		public List<BaseTowerAttackEffect> TowerAttackEffectConfigs => towerAttackEffectConfigs;
	}
}