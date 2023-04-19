using System.Linq;
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