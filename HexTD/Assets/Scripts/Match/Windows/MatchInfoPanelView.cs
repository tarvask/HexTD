using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows
{
    public class MatchInfoPanelView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentRoundWithTimer;
        [SerializeField] private TextMeshProUGUI nextRoundStartWithTimer;
        [SerializeField] private Image enemyCastleHealthBar;
        [SerializeField] private TextMeshProUGUI enemyCastleHealthText;
        [SerializeField] private Image ourCastleHealthBar;
        [SerializeField] private TextMeshProUGUI ourCastleHealthText;
        [SerializeField] private Text ourGoldCoinsCountText;
        [SerializeField] private Text ourCrystalsCountText;
        [SerializeField] private RawImage enemyFieldImage;

        [Space]
        [SerializeField] private MatchInfoRoundStartInfoPanelView roundStartInfoPanel;

        [Space]
        [SerializeField] private RectTransform middlePanelRect;

        public TextMeshProUGUI CurrentRoundWithTimer => currentRoundWithTimer;
        public TextMeshProUGUI NextRoundStartWithTimer => nextRoundStartWithTimer;
        public Image EnemyCastleHealthBar => enemyCastleHealthBar;
        public TextMeshProUGUI EnemyCastleHealthText => enemyCastleHealthText;
        public Image OurCastleHealthBar => ourCastleHealthBar;
        public TextMeshProUGUI OurCastleHealthText => ourCastleHealthText;
        public Text OurGoldCoinsCountText => ourGoldCoinsCountText;
        public Text OurCrystalsCountText => ourCrystalsCountText;
        public RawImage EnemyFieldImage => enemyFieldImage;

        public MatchInfoRoundStartInfoPanelView RoundStartInfoPanel => roundStartInfoPanel;

        public RectTransform MiddlePanelRect => middlePanelRect;
    }
}