using System;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
	[CreateAssetMenu(fileName = "TowerConfigNew", menuName = "Configs/Match/TowerConfigNew")]
	public class TowerConfigNew : ScriptableObject
	{
		[SerializeField] private string name;
		[SerializeField] private TowerView view;
		[SerializeField] private TowerLevelConfigsDictionary towerLevelConfigs;
		[SerializeField] private TowerAttackConfig towerAttackConfig;

		public string Name => name;
		public TowerView View => view;
		public TowerLevelConfigsDictionary TowerLevelConfigs => towerLevelConfigs;
		public TowerAttackConfig TowerAttackConfig => towerAttackConfig;
	}

	[Serializable]
	public class TowerLevelConfig
	{
		[SerializeField] private int buildPrice;
		[SerializeField] private int buildTime;

		public int BuildPrice => buildPrice;
		public int BuildTime => buildTime;
	}

	[Serializable]
	public class TowerLevelConfigsDictionary : UnitySerializedDictionary<int, TowerLevelConfig>
	{
	}
}