using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.AttackEffect
{
	[Serializable]
	public class AttacksConfig 
	{
		[SerializeField]
		private List<BaseSingleAttack> attacks;
		[SerializeField]
		private List<BaseSplashAttack> splashAttacks;

		public List<BaseSingleAttack> Attacks => attacks;
		public List<BaseSplashAttack> SplashAttacks => splashAttacks;
	}
}