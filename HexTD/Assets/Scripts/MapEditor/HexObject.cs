using HexSystem;
using UnityEngine;

namespace MapEditor
{
	public class HexObject : MonoBehaviour
	{
		[SerializeField] private GameObject isBlockerVisual;
		[SerializeField] private GameObject isRangeAttackBlockerVisual;
		[SerializeField] private GameObject isHighlightedVisual;
		
		private Hex2d hitHex;

		public Hex2d HitHex => hitHex;

		public void SetHex(Hex2d hex) => hitHex = hex;

		public void SetIsBlocker(bool isBlocker) => isBlockerVisual.SetActive(isBlocker);
		
		public void SetIsRangeAttackBlocker(bool isBlocker) => isRangeAttackBlockerVisual.SetActive(isBlocker);

		public void SetIsHighlighted(bool isHighlighted) => isHighlightedVisual.SetActive(isHighlighted);
	}
}