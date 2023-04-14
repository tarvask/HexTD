using System.Collections.Generic;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Tools;
using UniRx;

namespace Match.Windows.MainMenu
{
    public class StartGamePanelController : BaseDisposable
    {
        public struct Context
        {
            public StartGamePanelView View { get; }
            public ReactiveProperty<byte> SelectedHandIndexReactiveProperty { get; }
            public ReactiveCommand ResumeMultiPlayerGameButtonClickedReactiveCommand { get; }
            public ReactiveCommand StartSinglePlayerGameButtonClickedReactiveCommand { get; }
            public ReactiveCommand StartMultiPlayerGameButtonClickedReactiveCommand { get; }
            public ReactiveCommand ChooseLevelButtonClickedReactiveCommand { get; }
            public ReactiveCommand<string> LevelNameSelectedReactiveCommand { get; }

            public Context(StartGamePanelView view,
                ReactiveProperty<byte> selectedHandIndexReactiveProperty,
                ReactiveCommand resumeMultiPlayerGameButtonClickedReactiveCommand,
                ReactiveCommand startSinglePlayerGameButtonClickedReactiveCommand,
                ReactiveCommand startMultiPlayerGameButtonClickedReactiveCommand,
                ReactiveCommand chooseLevelButtonClickedReactiveCommand,
                ReactiveCommand<string> levelNameSelectedReactiveCommand)
            {
                View = view;
                SelectedHandIndexReactiveProperty = selectedHandIndexReactiveProperty;
                ResumeMultiPlayerGameButtonClickedReactiveCommand = resumeMultiPlayerGameButtonClickedReactiveCommand;
                StartSinglePlayerGameButtonClickedReactiveCommand = startSinglePlayerGameButtonClickedReactiveCommand;
                StartMultiPlayerGameButtonClickedReactiveCommand = startMultiPlayerGameButtonClickedReactiveCommand;
                ChooseLevelButtonClickedReactiveCommand = chooseLevelButtonClickedReactiveCommand;
                LevelNameSelectedReactiveCommand = levelNameSelectedReactiveCommand;
            }
        }

        private readonly Context _context;
        private List<List<TowerConfigNew>> _hands;

        public StartGamePanelController(Context context)
        {
            _context = context;
            
            for (byte handSwitchIndex = 0; handSwitchIndex < _context.View.HandsSwitchingButtons.Length; handSwitchIndex++)
            {
                byte index = handSwitchIndex;
                _context.View.HandsSwitchingButtons[handSwitchIndex].onClick.AddListener(() => SelectHand(index));
            }
            
            _context.View.ResumeMultiPlayerGameButton.onClick.AddListener(() => _context.ResumeMultiPlayerGameButtonClickedReactiveCommand.Execute());
            _context.View.StartSinglePlayerGameButton.onClick.AddListener(() => _context.StartSinglePlayerGameButtonClickedReactiveCommand.Execute());
            _context.View.StartMultiPlayerGameButton.onClick.AddListener(() => _context.StartMultiPlayerGameButtonClickedReactiveCommand.Execute());
            _context.View.ChooseLevelButton.onClick.AddListener(() => _context.ChooseLevelButtonClickedReactiveCommand.Execute());

            _context.LevelNameSelectedReactiveCommand.Subscribe(UpdateLevelName);
        }

        public void Show(List<List<TowerConfigNew>> hands, bool showResumeGameButton)
        {
            _hands = hands;
            SelectHand(_context.SelectedHandIndexReactiveProperty.Value, true);
            _context.View.gameObject.SetActive(true);
            _context.View.ResumeMultiPlayerGameButtonBlockerGo.SetActive(!showResumeGameButton);
        }
        
        public void Hide()
        {
            _context.View.gameObject.SetActive(false);
        }
        
        private void RefreshTowersInHand()
        {
            for (int towerIndex = 0; towerIndex < _hands[_context.SelectedHandIndexReactiveProperty.Value].Count; towerIndex++)
            {
                if (_hands[_context.SelectedHandIndexReactiveProperty.Value][towerIndex] != null)
                    _context.View.HandTowerItems[towerIndex].SetData(_hands[_context.SelectedHandIndexReactiveProperty.Value][towerIndex].RegularParameters);
                else
                    _context.View.HandTowerItems[towerIndex].SetData(null);
            }
        }
        
        private void SelectHand(byte handIndex, bool forceRefresh = false)
        {
            if (_context.SelectedHandIndexReactiveProperty.Value == handIndex && !forceRefresh)
                return;

            _context.SelectedHandIndexReactiveProperty.Value = handIndex;
            RefreshTowersInHand();

            for (int handSwitchIndex = 0; handSwitchIndex < _context.View.HandsSwitchingButtons.Length; handSwitchIndex++)
                _context.View.HandsSwitchingButtons[handSwitchIndex].interactable = (handSwitchIndex != handIndex);
        }

        private void UpdateLevelName(string newLevel)
        {
            _context.View.LevelNameText.text = newLevel;
        }
    }
}