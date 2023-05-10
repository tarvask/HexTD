using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.AttackEffect
{
	[Serializable]
	public class AttacksConfig 
	{
		[SerializeField]
		private List<BaseAttackEffect> attacks;
		[SerializeField]
		private List<BaseSplashAttack> splashAttacks;

		public List<BaseAttackEffect> Attacks => attacks;
		public List<BaseSplashAttack> SplashAttacks => splashAttacks;
	}
}