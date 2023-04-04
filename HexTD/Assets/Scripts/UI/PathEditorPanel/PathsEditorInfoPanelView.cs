using UnityEngine;
using UnityEngine.UI;

namespace UI.PathEditorPanel
{
    public class PathsEditorInfoPanelView : MonoBehaviour
    {
        [SerializeField] private PathEditInfoView pathEditInfoPrefab;
        [SerializeField] private Transform pathEditInfoTransformParent;
        [SerializeField] private Button addPathButton;

        public PathEditInfoView PathEditInfoPrefab => pathEditInfoPrefab;
        public Transform PathEditInfoTransformParent => pathEditInfoTransformParent;
        public Button AddPathButton => addPathButton;
    }
}