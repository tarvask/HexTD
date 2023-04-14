using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match.Field.Tower.TowerConfigs
{
	[Serializable]
	public class TowerAttackConfig 
	{
		[FormerlySerializedAs("towerAttackEffectConfigs")] [SerializeReference] [InlineEditor] 
		private List<BaseTowerAttack> towerAttacks;

		public List<BaseTowerAttack> TowerAttacks => towerAttacks;
	}
}