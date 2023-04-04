using UnityEngine;
using UnityEngine.UI;

namespace UI.InfoPanel
{
    public class EditorInfoPanelView : MonoBehaviour
    {
        [SerializeField] private Button infoSwitchButton;
        [SerializeField] private GameObject infoPanelGO;

        public Button InfoSwitchButton => infoSwitchButton;
        public GameObject InfoPanelGO => infoPanelGO;
    }
}