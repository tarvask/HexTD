using System.Collections.Generic;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Match.Windows.MainMenu;
using Photon.Pun.UtilityScripts;
using Services;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : BaseDisposable
{
    public struct Context
    {
        public MainMenuView View { get; }

        public Context(MainMenuView view)
        {
            View = view;
        }
    }

    private const string SinglePlayerGameStartSceneName = "StandaloneMatch";
    private const string MultiPlayerStartSceneName = "PhotonLobby";
    private const byte HandsNumber = 5;
    private const byte HandSize = 8;
    private readonly Context _context;

    private readonly ReactiveProperty<byte> _selectedHandIndexReactiveProperty;
    private List<List<TowerConfigNew>> _possibleHands;
    private readonly ConfigsRetriever _configsRetriever;
    private readonly MainMenuPanelController _mainMenuPanelController;
    private readonly StartGamePanelController _startGamePanelController;
    private readonly PlayerHandSelectionPanelController _playerHandSelectionPanelController;
    private readonly ChooseLevelPanelController _chooseLevelPanelController;

    private List<TowerConfigNew> SelectedHand => _possibleHands[_selectedHandIndexReactiveProperty.Value];

    public MainMenuController(Context context)
    {
        _context = context;

        // create hands
        _selectedHandIndexReactiveProperty = AddDisposable(new ReactiveProperty<byte>(0));
        _possibleHands = new List<List<TowerConfigNew>>();
        for (int handIndex = 0; handIndex < HandsNumber; handIndex++)
        {
            _possibleHands.Add(new List<TowerConfigNew>(HandSize));
            for (int itemIndex = 0; itemIndex < HandSize; itemIndex++)
                _possibleHands[handIndex].Add( null);
        }

        ConfigsRetriever.Context configsRetrieverContext = new ConfigsRetriever.Context(_context.View.FieldConfig);
        _configsRetriever = AddDisposable(new ConfigsRetriever(configsRetrieverContext));
            
        LoadHands();
            
        // commands and windows
        ReactiveCommand gameButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand towersButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand resumeMultiPlayerGameButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand startSinglePlayerGameButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand startMultiPlayerGameButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand chooseLevelButtonClickedReactiveCommand = AddDisposable(new ReactiveCommand());
        ReactiveCommand<int> levelIndexSelectedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
        ReactiveCommand<string> levelNameSelectedReactiveCommand = AddDisposable(new ReactiveCommand<string>());
            
        MainMenuPanelController.Context mainMenuPanelControllerContext = new MainMenuPanelController.Context(
            _context.View.MainMenuPanelView, gameButtonClickedReactiveCommand, towersButtonClickedReactiveCommand);
        _mainMenuPanelController = new MainMenuPanelController(mainMenuPanelControllerContext);
            
        StartGamePanelController.Context startGamePanelControllerContext = new StartGamePanelController.Context(
            _context.View.StartGamePanelView,
            _selectedHandIndexReactiveProperty,
            resumeMultiPlayerGameButtonClickedReactiveCommand,
            startSinglePlayerGameButtonClickedReactiveCommand,
            startMultiPlayerGameButtonClickedReactiveCommand,
            chooseLevelButtonClickedReactiveCommand,
            levelNameSelectedReactiveCommand);
        _startGamePanelController = AddDisposable(new StartGamePanelController(startGamePanelControllerContext));
            
        PlayerHandSelectionPanelController.Context playerHandSelectionPanelControllerContext = new PlayerHandSelectionPanelController.Context(
            _context.View.PlayerHandSelectionPanelView, _context.View.FieldConfig, _configsRetriever, _selectedHandIndexReactiveProperty);
        _playerHandSelectionPanelController = AddDisposable(new PlayerHandSelectionPanelController(playerHandSelectionPanelControllerContext));
        _startGamePanelController.Show(_possibleHands, PhotonRoomNameSaver.HasCachedGame);
            
        ChooseLevelPanelController.Context chooseLevelWindowControllerContext = new ChooseLevelPanelController.Context(
            _context.View.LevelsConfig, _context.View.ChooseLevelPanelView, levelIndexSelectedReactiveCommand);
        _chooseLevelPanelController = AddDisposable(new ChooseLevelPanelController(chooseLevelWindowControllerContext));

        // subscriptions
        gameButtonClickedReactiveCommand.Subscribe(GameButtonClickedEventHandler);
        towersButtonClickedReactiveCommand.Subscribe(TowersButtonClickedEventHandler);
        chooseLevelButtonClickedReactiveCommand.Subscribe(ChooseLevelButtonClickedHandler);

        resumeMultiPlayerGameButtonClickedReactiveCommand.Subscribe(ResumeMultiPlayerGameButtonClickedEventHandler);
        startSinglePlayerGameButtonClickedReactiveCommand.Subscribe(StartSinglePlayerGameButtonClickedEventHandler);
        startMultiPlayerGameButtonClickedReactiveCommand.Subscribe(StartMultiPlayerGameButtonClickedEventHandler);
        levelIndexSelectedReactiveCommand.Subscribe((levelIndex) =>
        {
            PlayerPrefs.SetInt("Level", levelIndex);
                
            levelNameSelectedReactiveCommand.Execute(_context.View.LevelsConfig.Levels[(byte)levelIndex].name);
            _chooseLevelPanelController.Hide();
        });
            
        byte currentLevelIndex = (byte)Mathf.Clamp(PlayerPrefs.GetInt("Level", 0), 0, _context.View.LevelsConfig.Levels.Count - 1);
        levelIndexSelectedReactiveCommand.Execute(currentLevelIndex);
            
        SceneManager.sceneUnloaded += (scene) => ShowMainMenu();
    }

    private void LoadHands()
    {
        for (int handIndex = 0; handIndex < HandsNumber; handIndex++)
        {
            for (int itemIndex = 0; itemIndex < HandSize; itemIndex++)
            {
                TowerType towerType = (TowerType)PlayerPrefs.GetInt($"Hand{handIndex}Item{itemIndex}", (int)TowerType.Undefined);
                _possibleHands[handIndex][itemIndex] = (towerType != TowerType.Undefined)
                    ? _configsRetriever.GetTowerByType(towerType)
                    : null;
            }
        }
    }

    private void SendHands()
    {
        for (int handIndex = 0; handIndex < HandsNumber; handIndex++)
        {
            for (int itemIndex = 0; itemIndex < HandSize; itemIndex++)
            {
                TowerType towerType = _possibleHands[handIndex][itemIndex] ?
                    _possibleHands[handIndex][itemIndex].RegularParameters.TowerType
                    : TowerType.Undefined;
                PlayerPrefs.SetInt($"Hand{handIndex}Item{itemIndex}", (int)towerType);
            }
        }
            
        // selected hand
        for (int itemIndex = 0; itemIndex < HandSize; itemIndex++)
        {
            TowerType towerType = SelectedHand[itemIndex] ?
                SelectedHand[itemIndex].RegularParameters.TowerType
                : TowerType.Undefined;
            PlayerPrefs.SetInt($"CurrentHandItem{itemIndex}", (int)towerType);
        }
    }

    private void GameButtonClickedEventHandler(Unit unit)
    {
        _startGamePanelController.Show(_possibleHands, PhotonRoomNameSaver.HasCachedGame);
        _playerHandSelectionPanelController.Hide();
    }

    private void TowersButtonClickedEventHandler(Unit unit)
    {
        _startGamePanelController.Hide();
        _playerHandSelectionPanelController.Show(ref _possibleHands);
    }

    private void ResumeMultiPlayerGameButtonClickedEventHandler(Unit unit)
    {
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(MultiPlayerStartSceneName, LoadSceneMode.Additive);
        HideMainMenu();
    }

    private void StartSinglePlayerGameButtonClickedEventHandler(Unit unit)
    {
        PhotonRoomNameSaver.DropLastGameRoomName();
        SendHands();
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(SinglePlayerGameStartSceneName, LoadSceneMode.Additive);
        HideMainMenu();
    }
        
    private void StartMultiPlayerGameButtonClickedEventHandler(Unit unit)
    {
        PhotonRoomNameSaver.DropLastGameRoomName();
        SendHands();
        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(MultiPlayerStartSceneName, LoadSceneMode.Additive);
        HideMainMenu();
    }

    private void ChooseLevelButtonClickedHandler(Unit unit)
    {
        _chooseLevelPanelController.Show();
    }

    private void ShowMainMenu()
    {
        _startGamePanelController.Show(_possibleHands, PhotonRoomNameSaver.HasCachedGame);
        _context.View.MainMenuPanelView.gameObject.SetActive(true);
    }

    private void HideMainMenu()
    {
        _startGamePanelController.Hide();
        _playerHandSelectionPanelController.Hide();
        _context.View.MainMenuPanelView.gameObject.SetActive(false);
    }
}