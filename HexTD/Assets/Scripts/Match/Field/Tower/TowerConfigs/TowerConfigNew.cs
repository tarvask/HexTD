using System;
using Match.Field.AttackEffect;
using Tools;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
	[CreateAssetMenu(fileName = "TowerConfigNew", menuName = "Configs/Match/TowerConfigNew")]
	public class TowerConfigNew : ScriptableObject
	{
		public const int FirstTowerLevel = 1;
		
		[SerializeField] private Sprite icon;
		[SerializeField] private TowerView view;
		[SerializeField] private TowerRegularParameters regularParameters;
		[SerializeField] private TowerLevelConfigsDictionary towerLevelConfigs;
		[SerializeField] private AttacksConfig attacksConfig;

		public Sprite Icon => icon;
		public TowerView View => view;
		public TowerRegularParameters RegularParameters => regularParameters;
		public TowerLevelConfigsDictionary TowerLevelConfigs => towerLevelConfigs;
		public AttacksConfig AttacksConfig => attacksConfig;
	}

	[Serializable]
	public class TowerLevelConfig
	{
		[SerializeField] private int buildPrice;
		[SerializeField] private int refundPrice;
		[SerializeField] private int buildTime;

		[SerializeField] private float healthPoint;
		
		public int BuildPrice => buildPrice;
		public int RefundPrice => refundPrice;
		public int BuildTime => buildTime;
		public float HealthPoint => healthPoint;
	}

	[Serializable]
	public class TowerLevelConfigsDictionary : UnitySerializedDictionary<int, TowerLevelConfig>
	{
	}
}