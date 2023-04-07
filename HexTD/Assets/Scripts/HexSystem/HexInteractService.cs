using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexInteractService
    {
        private readonly Camera _mainCamera;
        private readonly Layout _layout;
        private readonly Plane _touchDetectionPlane;

        public HexInteractService(Layout layout,
            Camera mainCamera)
        {
            _layout = layout;
            _mainCamera = mainCamera;
            _touchDetectionPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public bool TryGetHexUnderPointer(out Hex2d hitHex)
        {
            if (TryClickHexTile(out hitHex))
                return true;
            
            if (TryClickHexPlane(out hitHex))
                return true;

            hitHex = new Hex2d();
            return false;
        }

        public bool TryClickHexTile(out Hex2d hitHex)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

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

        public bool TryClickHexPlane(out Hex2d hitHex)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (_touchDetectionPlane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                
                var x = _layout.ToHex(hitPoint).RoundToHex();
                hitHex = new Hex2d(x.Q,x.R);
                return true;
            }

            hitHex = new Hex2d();
            return false;
        }
        
        void DrawHitPoint(Vector3 hitPoint) => Debug.DrawRay(hitPoint, Vector3.up, Color.yellow, 1.0f);
    }
}