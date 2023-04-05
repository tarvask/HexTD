using Match.Windows.Tower;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class PlayerHandSelectionPanelView : MonoBehaviour
    {
        [SerializeField] private PlayerHandSelectionPossibleItemView[] handTowerItems;
        [SerializeField] private Button[] handsSwitchingButtons;
        [SerializeField] private ScrollRect towerItemsScroll;
        [SerializeField] private RectTransform towerItemsRoot;
        [SerializeField] private RectTransform towerWaterItemsRoot;
        [SerializeField] private RectTransform towerFireItemsRoot;
        [SerializeField] private RectTransform towerNatureItemsRoot;
        [SerializeField] private RectTransform towerEarthItemsRoot;
        [SerializeField] private RectTransform towerDeathItemsRoot;
        
        [SerializeField] private Button towerItemMenuCloseButton;
        [SerializeField] private PossibleTowerItemMenuView towerItemMenu;
        [SerializeField] private PlayerHandTowerSelectionWindowView playerHandTowerSelectionWindowView;
        [SerializeField] private TowerInfoWindowView towerInfoWindowView;

        [SerializeField] private PlayerHandSelectionPossibleItemView possibleTowerItemPrefab;

        public PlayerHandSelectionPossibleItemView[] HandTowerItems => handTowerItems;
        public Button[] HandsSwitchingButtons => handsSwitchingButtons;
        public ScrollRect TowerItemsScroll => towerItemsScroll;
        public RectTransform TowerItemsRoot => towerItemsRoot;
        public RectTransform TowerWaterItemsRoot => towerWaterItemsRoot;
        public RectTransform TowerFireItemsRoot => towerFireItemsRoot;
        public RectTransform TowerNatureItemsRoot => towerNatureItemsRoot;
        public RectTransform TowerEarthItemsRoot => towerEarthItemsRoot;
        public RectTransform TowerDeathItemsRoot => towerDeathItemsRoot;

        public Button TowerItemMenuCloseButton => towerItemMenuCloseButton;
        public PossibleTowerItemMenuView TowerItemMenu => towerItemMenu;
        public PlayerHandTowerSelectionWindowView PlayerHandTowerSelectionWindowView => playerHandTowerSelectionWindowView;
        public TowerInfoWindowView TowerInfoWindowView => towerInfoWindowView;

        public PlayerHandSelectionPossibleItemView PossibleTowerItemPrefab => possibleTowerItemPrefab;
    }
}