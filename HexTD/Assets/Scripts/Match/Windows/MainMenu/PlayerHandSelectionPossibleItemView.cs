using Match.Field.Mob;
using Match.Field.Tower;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class PlayerHandSelectionPossibleItemView : BaseMonoBehaviour
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private Image raceIcon;
        [SerializeField] private TextMeshProUGUI towerNameText;
        [SerializeField] private Color[] raceColors;

        public Button SelectButton => selectButton;
        public Image RaceIcon => raceIcon;
        public TextMeshProUGUI TowerNameText => towerNameText;
        public Color[] RaceColors => raceColors;

        public void SetData(TowerRegularParameters towerParameters)
        {
            if (towerParameters != null)
            {
                towerNameText.text = towerParameters.TowerName;
                raceIcon.color = raceColors[(int) towerParameters.RaceType];
            }
            else
            {
                // default values
                towerNameText.text = "";
                raceIcon.color = raceColors[(int) RaceType.Undefined];
            }
        }
    }
}