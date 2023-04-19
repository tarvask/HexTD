using Lean.Touch;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexInteractService
    {
        protected readonly Camera MainCamera;   

        public HexInteractService(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public bool TryClickHexTile(LeanFinger finger, out Hex2d hitHex)
        {
            var ray = finger.GetRay();

            if (Physics.Raycast(ray, out var hitInfo))
            {
                var hexObject = hitInfo.transform.GetComponent<HexObject>();
                if (hexObject != null)
                {
                    hitHex = hexObject.HitHex;
                    return true;
                }
            }

            hitHex = new Hex2d();
            return false;
        }
    }
}