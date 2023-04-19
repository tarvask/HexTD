using HexSystem;
using UnityEngine;

namespace MapEditor
{
	public class HexObject : MonoBehaviour
	{
		[SerializeField] private GameObject isBlockerVisual;
		
		private Hex2d hitHex;

		public Hex2d HitHex => hitHex;

		public void SetHex(Hex2d hex) => hitHex = hex;

		public void SetIsBlocker(bool isBlocker) => isBlockerVisual.SetActive(isBlocker);
	}
}