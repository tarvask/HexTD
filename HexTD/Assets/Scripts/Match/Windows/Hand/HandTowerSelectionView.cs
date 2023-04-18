using TMPro;
using Tools;
using UnityEngine;

namespace Match.Windows.Hand
{
    public class HandTowerSelectionView : BaseMonoBehaviour
    {
        [SerializeField] private Transform towerCardRoot;
        [SerializeField] private TowerCardView towerCardView;
        [SerializeField] private TMP_Text energyCountText;

        public Transform TowerCardRoot => towerCardRoot;
        public TowerCardView TowerCardView => towerCardView;
        public TMP_Text EnergyCountText => energyCountText;
    }
}