using HexSystem;
using Lean.Touch;
using UnityEngine;

namespace MapEditor
{
    public class EditorHexInteractService : HexInteractService
    {
        private readonly Layout _layout;
        private readonly Plane _touchDetectionPlane;

        public EditorHexInteractService(Layout layout,
            Camera mainCamera) : base(mainCamera)
        {
            _layout = layout;
            _touchDetectionPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public bool TryGetHexUnderPointer(LeanFinger finger, out Hex2d hitHex)
        {
            if (finger.IsOverGui)
            {
                hitHex = new Hex2d();                
                return false;
            }
            
            if (TryClickHexTile(finger, out hitHex))
                return true;
            
            if (TryClickHexPlane(finger, out hitHex))
                return true;

            hitHex = new Hex2d();
            return false;
        }

        public bool TryClickHexPlane(LeanFinger finger, out Hex2d hitHex)
        {
            var ray = finger.GetRay();
            
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
    }
}