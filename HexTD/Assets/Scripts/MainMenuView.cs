using Match;
using Match.Field.Tower;
using Match.Windows.MainMenu;
using Tools;
using UnityEngine;

public class MainMenuView : BaseMonoBehaviour
{
    [SerializeField] private MainMenuPanelView mainMenuPanelView;
        
    [SerializeField] private StartGamePanelView startGamePanelView;
    [SerializeField] private PlayerHandSelectionPanelView playerHandSelectionPanelView;
    [SerializeField] private ChooseLevelPanelView chooseLevelPanelView;

    [Space] [SerializeField] private MatchesConfig levelsConfig;
    [SerializeField] private TowersConfig towersConfig;

    public MainMenuPanelView MainMenuPanelView => mainMenuPanelView;
        
    public StartGamePanelView StartGamePanelView => startGamePanelView;
    public PlayerHandSelectionPanelView PlayerHandSelectionPanelView => playerHandSelectionPanelView;
    public ChooseLevelPanelView ChooseLevelPanelView => chooseLevelPanelView;

    public MatchesConfig LevelsConfig => levelsConfig;
    public TowersConfig TowersConfig => towersConfig;
}