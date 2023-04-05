using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class PlayerHandTowerSelectionWindowView : BaseMonoBehaviour
    {
        [SerializeField] private PlayerHandSelectionPossibleItemView[] towerSlots;
        [SerializeField] private PlayerHandSelectionPossibleItemView selectedTowerItem;
        [SerializeField] private Button backButton;

        public PlayerHandSelectionPossibleItemView[] TowerSlots => towerSlots;
        public PlayerHandSelectionPossibleItemView SelectedTowerItem => selectedTowerItem;
        public Button BackButton => backButton;
    }
}