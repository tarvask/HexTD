using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows
{
    public class MatchInfoRoundStartInfoPanelView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundNumberText;
        [SerializeField] private Image[] waveElements;
        [SerializeField] private Image[] reinforcementsItems;

        public TextMeshProUGUI RoundNumberText => roundNumberText;
        public Image[] WaveElements => waveElements;
        public Image[] ReinforcementsItems => reinforcementsItems;
    }
}