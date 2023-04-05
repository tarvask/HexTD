using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class ChooseLevelItemView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Button selectLevelButton;

        public TextMeshProUGUI LevelNameText => levelNameText;
        public Button SelectLevelButton => selectLevelButton;
    }
}