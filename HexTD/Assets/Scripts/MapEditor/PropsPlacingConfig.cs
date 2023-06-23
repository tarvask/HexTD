using System;
using UnityEngine;

namespace MapEditor
{
	[Serializable]
	public class PropsPlacingConfig
	{
//		[SerializeField] private bool isHeightSnap;
//		[SerializeField] private bool isGridSnap;
		[SerializeField] private bool isReplacesHex;
		[SerializeField] private bool isBuildingBlock;
		[SerializeField] private bool isRangeAttackBlock;

		public bool IsReplacesHex => isReplacesHex;
		public bool IsBuildingBlock => isBuildingBlock;
		public bool IsRangeAttackBlock => isRangeAttackBlock;
	}
}