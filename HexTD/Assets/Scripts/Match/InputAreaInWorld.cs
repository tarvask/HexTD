using Tools;
using UnityEngine;

namespace Match
{
    public class InputAreaInWorld : MonoBehaviour
    {
        [SerializeField] private Rect areaRect;

        public Rect AreaRect => areaRect;

        private void OnDrawGizmos()
        {
            Vector2 areaPosition = transform.position;
            GizmosTools.DrawRect(areaRect.min + areaPosition, areaRect.max + areaPosition);
        }
    }
}
