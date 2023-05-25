using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PathEditorPanel
{
    public class PathEditInfoView : MonoBehaviour
    {
        [SerializeField] private PointEditorPanelView pointEditorPanelPrefab;
        [SerializeField] private Transform pointsParent;
        [SerializeField] private TMP_InputField nameFieldText;
        [SerializeField] private Button selectPathButton;
        [SerializeField] private Button deletePathButton;
        [SerializeField] private GameObject selectSignObject;

        public PointEditorPanelView PointEditorPanelPrefab => pointEditorPanelPrefab;
        public Transform PointsParent => pointsParent;
        public TMP_InputField NameFieldText => nameFieldText;
        public Button SelectPathButton => selectPathButton;
        public Button DeletePathButton => deletePathButton;
        public GameObject SelectSignObject => selectSignObject; 
    }
}