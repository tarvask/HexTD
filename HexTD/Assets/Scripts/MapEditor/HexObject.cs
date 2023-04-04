using HexSystem;
using UnityEngine;

namespace MapEditor
{
	public class HexObject : MonoBehaviour
	{
		private Hex2d hitHex;

		public Hex2d HitHex => hitHex;

		public void SetHex(Hex2d hex) => hitHex = hex;
	}
}