using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexInteractService
    {
        private readonly Camera _editorCamera;
        private readonly Layout _layout;
        private readonly Plane _touchDetectionPlane;

        public HexInteractService(Layout layout,
            Camera editorCamera)
        {
            _layout = layout;
            _editorCamera = editorCamera;
            _touchDetectionPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public bool TryGetHexUnderPointer(out Hex2d hitHex)
        {
            var ray = _editorCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo))
            {
                var hexObject = hitInfo.transform.GetComponent<HexObject>();
                if (hexObject != null)
                {
                    hitHex = hexObject.HitHex;
                    return true;
                }
            }
            
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