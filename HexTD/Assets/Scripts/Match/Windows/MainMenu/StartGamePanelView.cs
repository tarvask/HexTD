using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.MainMenu
{
    public class StartGamePanelView : MonoBehaviour
    {
        [SerializeField] private PlayerHandSelectionPossibleItemView[] handTowerItems;
        [SerializeField] private Button[] handsSwitchingButtons;
        [SerializeField] private Button resumeMultiPlayerGameButton;
        [SerializeField] private GameObject resumeMultiPlayerGameButtonBlockerGo;
        [SerializeField] private Button startSinglePlayerGameButton;
        [SerializeField] private Button startMultiPlayerGameButton;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Button chooseLevelButton;

        public PlayerHandSelectionPossibleItemView[] HandTowerItems => handTowerItems;
        public Button[] HandsSwitchingButtons => handsSwitchingButtons;
        public Button ResumeMultiPlayerGameButton => resumeMultiPlayerGameButton;
        public GameObject ResumeMultiPlayerGameButtonBlockerGo => resumeMultiPlayerGameButtonBlockerGo;
        public Button StartSinglePlayerGameButton => startSinglePlayerGameButton;
        public Button StartMultiPlayerGameButton => startMultiPlayerGameButton;
        public TextMeshProUGUI LevelNameText => levelNameText;
        public Button ChooseLevelButton => chooseLevelButton;
    }
}