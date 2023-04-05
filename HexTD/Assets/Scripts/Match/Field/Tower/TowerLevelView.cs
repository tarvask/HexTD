using Tools;

namespace Match.Field.Tower
{
    //[RequireComponent(typeof(TowerLevelRegularParamsMarker))]
    public class TowerLevelView : BaseMonoBehaviour
    {
        private bool _isActive;

        public void SetActive(bool activate)
        {
            if (activate != _isActive)
            {
                gameObject.SetActive(activate);
                _isActive = activate;
            }
        }
    }
}