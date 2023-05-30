using TMPro;
using Tools;
using UnityEngine;

namespace Match.Windows.Tower
{
    public class TowerInfoPanelView : BaseMonoBehaviour
    {
        // regular params
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private TextMeshProUGUI towerDamageText;
        [SerializeField] private TextMeshProUGUI towerAttackRateText;
        [SerializeField] private TextMeshProUGUI towerRangeText;
        [SerializeField] private TextMeshProUGUI towerTargetText;
        
        // buffs
        [SerializeField] private Transform buffsInfoScrollRoot;
        [SerializeField] private TextMeshProUGUI buffInfoElementPrefab;
        
        // regular params
        public TextMeshProUGUI TowerNameText => towerNameText;
        public TextMeshProUGUI TowerDamageText => towerDamageText;
        public TextMeshProUGUI TowerAttackRateText => towerAttackRateText;
        public TextMeshProUGUI TowerRangeText => towerRangeText;
        public TextMeshProUGUI TowerTargetText => towerTargetText;
        
        // buffs
        public Transform BuffsInfoScrollRoot => buffsInfoScrollRoot;
        public TextMeshProUGUI BuffInfoElementPrefab => buffInfoElementPrefab;
    }
}