using UI.Tools;

namespace UI.EditorInfoPanel
{
    public class EditorInfoPanelController : BaseDisposable
    {
        private readonly EditorInfoPanelView _infoPanelView;
        private bool _isInfoActive;
        
        public EditorInfoPanelController(EditorInfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
            
            _infoPanelView.InfoSwitchButton.onClick.RemoveAllListeners();
            _infoPanelView.InfoSwitchButton.onClick.AddListener(OnSwitchInfo);
        }

        private void OnSwitchInfo()
        {
            _isInfoActive = !_isInfoActive;
            _infoPanelView.InfoPanelGO.SetActive(_isInfoActive);
        }
    }
}