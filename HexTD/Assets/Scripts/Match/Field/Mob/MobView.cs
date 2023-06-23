using Tools;
using UI.ScreenSpaceOverlaySystem;
using UnityEngine;

namespace Match.Field.Mob
{
	public class MobView : BaseMonoBehaviour, ITargetView
	{
		[SerializeField] private Transform infoPanelPivot;
		public Transform InfoPanelPivot => infoPanelPivot;
	}
}