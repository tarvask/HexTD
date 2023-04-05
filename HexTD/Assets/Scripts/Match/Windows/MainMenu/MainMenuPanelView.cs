using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class MainMenuPanelView : BaseMonoBehaviour
    {
        [SerializeField] private Button gameButton;
        [SerializeField] private Button towersButton;

        public Button GameButton => gameButton;
        public Button TowersButton => towersButton;
    }
}