using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class PossibleTowerItemMenuView : BaseMonoBehaviour
    {
        [SerializeField] private RectTransform menuRectTransform;
        [SerializeField] private PlayerHandSelectionPossibleItemView towerItem;
        [SerializeField] private Button useButton;
        [SerializeField] private Button infoButton;

        public RectTransform MenuRectTransform => menuRectTransform;
        public PlayerHandSelectionPossibleItemView TowerItem => towerItem;
        public Button UseButton => useButton;
        public Button InfoButton => infoButton;
    }
}