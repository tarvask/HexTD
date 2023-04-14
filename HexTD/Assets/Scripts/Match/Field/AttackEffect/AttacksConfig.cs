using System;
using System.Collections.Generic;
using Match.Field.Tower.TowerConfigs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Field.AttackEffect
{
	[Serializable]
	public class AttacksConfig 
	{
		[SerializeReference] [InlineEditor] 
		private List<BaseAttackEffect> attacks;

		public List<BaseAttackEffect> Attacks => attacks;
	}
}