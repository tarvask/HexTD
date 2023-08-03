using System.Linq;
using Lean.Touch;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexInteractService
    {
        public bool TryClickHexTile(LeanFinger finger, out Hex2d hitHex)
        {
            var ray = finger.GetRay();

            var raycasts = Physics.RaycastAll(ray);

            var hexObject = raycasts
                .Select(hit => hit.transform.GetComponentInParent<HexObject>())
                .FirstOrDefault(hexObject => hexObject != null);
            
            if (hexObject != null)
            {
                hitHex = hexObject.HitHex;
                return true;
            }

            hitHex = new Hex2d();
            return false;
        }
    }
}