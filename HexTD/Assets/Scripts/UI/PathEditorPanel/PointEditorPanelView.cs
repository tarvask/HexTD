using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PathEditorPanel
{
    public class PointEditorPanelView : MonoBehaviour
    {
        [SerializeField] private Button selectPointButton;
        [SerializeField] private TMP_Text pointPositionInfo;

        public Button SelectPointButton => selectPointButton;
        public TMP_Text PointPositionInfo => pointPositionInfo;
    }
}