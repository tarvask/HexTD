using UnityEngine;

namespace HexSystem
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

        public bool TryGetHexUnderPointer(out Hex2d hitHex)
        {
            if (TryClickHexTile(out hitHex))
                return true;
            
            if (TryClickHexPlane(out hitHex))
                return true;

            hitHex = new Hex2d();
            return false;
        }

        public bool TryClickHexPlane(out Hex2d hitHex)
        {
            var ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            
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