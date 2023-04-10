using TMPro;
using Tools;
using UnityEngine;

namespace Match.Windows
{
    public class MatchInfoRoundStartInfoPanelView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roundNumberText;

        public TextMeshProUGUI RoundNumberText => roundNumberText;
    }
}