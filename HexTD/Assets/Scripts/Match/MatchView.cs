using Match.Field;
using Services;
using Tools;
using UnityEngine;

namespace Match
{
    public class MatchView : BaseMonoBehaviour
    {
        [SerializeField] private FieldView enemyFieldView;
        [SerializeField] private FieldView ourFieldView;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasHeightOrWidthFitter canvasFitter;
        [SerializeField] private MatchCanvasBuilder canvasBuilder;

        [SerializeField] private FieldConfig fieldConfig;

        [SerializeField] private MatchUiViewsCollection matchUiViews;

        public FieldView EnemyFieldView => enemyFieldView;
        public FieldView OurFieldView => ourFieldView;
        public Camera MainCamera => mainCamera;
        public Canvas Canvas => canvas;
        public CanvasHeightOrWidthFitter CanvasFitter => canvasFitter;
        public MatchCanvasBuilder CanvasBuilder => canvasBuilder;

        public FieldConfig FieldConfig => fieldConfig;

        public MatchUiViewsCollection MatchUiViews => matchUiViews;
    }
}
