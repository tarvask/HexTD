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

        public bool TryClickHexTile(out Hex2d hitHex)
        {
            var ray = MainCamera.ScreenPointToRay(Input.mousePosition);

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