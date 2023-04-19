using Match.Field;
using Tools;
using UnityEngine;

namespace Match
{
    public class MatchView : BaseMonoBehaviour
    {
        [SerializeField] private Transform ourFieldRoot;
        [SerializeField] private Transform enemyFieldRoot;
        [SerializeField] private Camera ourFieldCamera;
        [SerializeField] private Camera enemyFieldCamera;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasHeightOrWidthFitter canvasFitter;

        [SerializeField] private FieldConfig fieldConfig;

        [SerializeField] private MatchUiViewsCollection matchUiViews;

        public Transform OurFieldRoot => ourFieldRoot;
        public Transform EnemyFieldRoot => enemyFieldRoot;
        public Camera OurFieldCamera => ourFieldCamera;
        public Camera EnemyFieldCamera => enemyFieldCamera;
        public Canvas Canvas => canvas;
        public CanvasHeightOrWidthFitter CanvasFitter => canvasFitter;

        public FieldConfig FieldConfig => fieldConfig;

        public MatchUiViewsCollection MatchUiViews => matchUiViews;
    }
}
