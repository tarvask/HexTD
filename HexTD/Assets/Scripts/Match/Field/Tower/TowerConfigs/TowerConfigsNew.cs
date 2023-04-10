using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
	[CreateAssetMenu(fileName = "TowerConfigsNew", menuName = "Configs/Match/TowerConfigsNew")]
	public class TowerConfigsNew : ScriptableObject
	{
		[SerializeField] [InlineEditor] private List<TowerConfigNew> towers;

		public List<TowerConfigNew> Towers => towers;
	}
}