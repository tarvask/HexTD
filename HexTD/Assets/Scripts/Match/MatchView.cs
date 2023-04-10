using Match.Field;
using Tools;
using UnityEngine;

namespace Match
{
    public class MatchView : BaseMonoBehaviour
    {
        [SerializeField] private Transform ourFieldRoot;
        [SerializeField] private Transform enemyFieldRoot;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasHeightOrWidthFitter canvasFitter;

        [SerializeField] private FieldConfig fieldConfig;

        [SerializeField] private MatchUiViewsCollection matchUiViews;

        public Transform OurFieldRoot => ourFieldRoot;
        public Transform EnemyFieldRoot => enemyFieldRoot;
        public Camera MainCamera => mainCamera;
        public Canvas Canvas => canvas;
        public CanvasHeightOrWidthFitter CanvasFitter => canvasFitter;

        public FieldConfig FieldConfig => fieldConfig;

        public MatchUiViewsCollection MatchUiViews => matchUiViews;
    }
}
