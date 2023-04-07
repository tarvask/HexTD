using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows
{
    public class MatchInfoRoundStartInfoPanelView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundNumberText;

        public TextMeshProUGUI RoundNumberText => roundNumberText;
    }
}