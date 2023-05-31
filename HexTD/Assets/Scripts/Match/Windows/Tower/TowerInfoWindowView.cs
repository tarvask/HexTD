using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.Tower
{
    public class TowerInfoWindowView : BaseWindowView
    {
        [SerializeField] private Button closeButton;
        
        [SerializeField] private TowerInfoPanelView infoPanel;

        public Button CloseButton => closeButton;

        public TowerInfoPanelView InfoPanel => infoPanel;
    }
}