using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.Tower
{
    public class TowerManipulationWindowView : BaseWindowView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI upgradePriceText;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private TextMeshProUGUI sellPriceText;

        public Button CloseButton => closeButton;
        public Button UpgradeButton => upgradeButton;
        public TextMeshProUGUI UpgradePriceText => upgradePriceText;
        public Button InfoButton => infoButton;
        public Button SellButton => sellButton;
        public TextMeshProUGUI SellPriceText => sellPriceText;
    }
}
