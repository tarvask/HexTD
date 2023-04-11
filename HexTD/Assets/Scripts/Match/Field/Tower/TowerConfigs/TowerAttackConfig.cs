using System;
using System.Collections.Generic;
using Match.Field.Shooting.TargetFinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
	[Serializable]
	public class TowerAttackConfig
	{
		[SerializeField] private float baseDamage;
		[SerializeField] private float cooldown;
		[SerializeField] private TargetFindingTacticType targetFindingTacticType;
		[SerializeField] private GameObject projectileView;
		[SerializeReference] [InlineEditor] private List<ITowerAttackEffectConfig> towerAttackEffectConfigs;

		public float BaseDamage => baseDamage;
		public float Cooldown => cooldown;
		public TargetFindingTacticType TargetFindingTacticType => targetFindingTacticType;
		public GameObject ProjectileView => projectileView;
		public List<ITowerAttackEffectConfig> TowerAttackEffectConfigs => towerAttackEffectConfigs;
	}
}