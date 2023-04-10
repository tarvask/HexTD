using Match.Field;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match
{
    public class MatchView : BaseMonoBehaviour
    {
        [SerializeField] private Transform fieldRoot;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasHeightOrWidthFitter canvasFitter;

        [SerializeField] private FieldConfig fieldConfig;

        [SerializeField] private MatchUiViewsCollection matchUiViews;

        public Transform FieldRoot => fieldRoot;
        public Camera MainCamera => mainCamera;
        public Canvas Canvas => canvas;
        public CanvasHeightOrWidthFitter CanvasFitter => canvasFitter;

        public FieldConfig FieldConfig => fieldConfig;

        public MatchUiViewsCollection MatchUiViews => matchUiViews;
    }
}
