using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.Tower
{
    public class TowerSelectionWindowView : BaseWindowView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private TowerItemView towerItemPrefab;

        public Button CloseButton => closeButton;
        public Transform ContentRoot => contentRoot;
        public TowerItemView TowerItemPrefab => towerItemPrefab;
    }
}