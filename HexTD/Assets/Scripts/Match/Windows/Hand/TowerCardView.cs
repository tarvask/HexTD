using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.Hand
{
    public class TowerCardView : BaseMonoBehaviour
    {
        [SerializeField] private Transform readyBgImage;
        [SerializeField] private Transform notReadyBgImage;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text towerNameText;
        
        public Transform ReadyBgImage => readyBgImage;
        public Transform NotReadyBgImage => notReadyBgImage;
        public Button Button => button;
        public TMP_Text CostText => costText;
        public TMP_Text TowerNameText => towerNameText;
    }
}