using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows
{
    public class MobInfoWindowView : BaseWindowView
    {
        [SerializeField] private Button closeButton;
        
        [SerializeField] private TextMeshProUGUI mobNameText;
        [SerializeField] private TextMeshProUGUI mobPowerText;
        [SerializeField] private Image mobIcon;
        
        [SerializeField] private TextMeshProUGUI mobPriceText;
        [SerializeField] private TextMeshProUGUI mobIncomeText;
        [SerializeField] private TextMeshProUGUI mobHealthText;
        [SerializeField] private TextMeshProUGUI mobAttackPowerText;
        [SerializeField] private TextMeshProUGUI mobAttackRateText;
        [SerializeField] private TextMeshProUGUI mobSpeedText;
        [SerializeField] private TextMeshProUGUI mobRewardText;

        [SerializeField] private TextMeshProUGUI mobAbilitiesDescription;

        public Button CloseButton => closeButton;
        
        public TextMeshProUGUI MobNameText => mobNameText;
        public TextMeshProUGUI MobPowerText => mobPowerText;
        public Image MobIcon => mobIcon;

        public TextMeshProUGUI MobPriceText => mobPriceText;
        public TextMeshProUGUI MobIncomeText => mobIncomeText;
        public TextMeshProUGUI MobHealthText => mobHealthText;
        public TextMeshProUGUI MobAttackPowerText => mobAttackPowerText;
        public TextMeshProUGUI MobAttackRateText => mobAttackRateText;
        public TextMeshProUGUI MobSpeedText => mobSpeedText;
        public TextMeshProUGUI MobRewardText => mobRewardText;

        public TextMeshProUGUI MobAbilitiesDescription => mobAbilitiesDescription;
    }
}