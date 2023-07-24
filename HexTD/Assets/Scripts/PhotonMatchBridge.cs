using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match;
using Match.Commands;
using Match.EventBus;
using Match.Field.Hand;
using Match.Field.Tower;
using Photon.Pun;
using Photon.Realtime;
using Services;
using Services.PhotonRelated;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

public class PhotonMatchBridge : BaseMonoBehaviour
{
    public IObservable<Unit> OnQuitMatch => _onQuitMatch;
    private Subject<Unit> _onQuitMatch = new Subject<Unit>();
    public IObservable<Unit> OnEndMatch => _onEndMatch;
    private Subject<Unit> _onEndMatch = new Subject<Unit>();

    [SerializeField] private MatchesConfig levelsConfig;

    private const bool HasAuthorityServer = false;
    private const int EmptyRoomTimeToLive = 10000;
        
    private TestMatchEngine _matchEngine;
    private ConnectionMaintainer _connectionMaintainer;
    private AbstractEventBus _eventBus;
    private ProcessRolesDefiner _processRolesDefiner;
    private NetworkMatchStatus _networkMatchStatus;
    private CommandExecutorsAggregator _commandExecutorsAggregator;
    private PingDamper _pingDamper;

    private List<Player> _players;
    private bool _isDisposed;
    private bool _isMultiPlayerGame;

    private bool IsServer => !_isMultiPlayerGame || PhotonNetwork.IsMasterClient;

    [Inject]
    public void Constructor(MatchSettingsProvider matchSettingsProvider)
    {
        _isMultiPlayerGame = matchSettingsProvider.Settings.IsMultiPlayer;
        Debug.Log($"{nameof(_isMultiPlayerGame)}: {_isMultiPlayerGame}");
    }
    
    private async void Start()
    {
        await InitMatch();
    }

    private void Update()
    {
        _pingDamper.UpdatePingTimeout(_players);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
            
        if (!_isDisposed)
            Dispose();
    }
        
    private async Task InitMatch()
    {
        _pingDamper = new PingDamper();
        // maybe check here for lowest ActorNumber in room
        _connectionMaintainer = FindObjectOfType<ConnectionMaintainer>();
        _connectionMaintainer.OnRequestStateEvent += RequestState;
        _connectionMaintainer.OnRollbackStateEvent += RollbackState;

        await Task.Run(WaitForFullRoom);
        Debug.Log("Room is full");
            
        if (_isMultiPlayerGame)
            SendPlayerHand();
            
        _players = new List<Player>(PhotonNetwork.PlayerList);
            
        if (IsServer)
        {
            await Task.Delay(3000);
            CreateNetworkMatchStatus();
            CreateEventBus();
            await InitAsServer();
        }
        else
            CreateEventBus();
            
        if (_isMultiPlayerGame && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PhotonEventsConstants.MatchStarted))
            ResumeGame();
    }

    private async Task WaitForFullRoom()
    {
        if (_isMultiPlayerGame)
        {
            if (HasAuthorityServer)
            {
                while (!PhotonNetwork.IsConnectedAndReady)
                    await Task.Delay(100);
                while (PhotonNetwork.CurrentRoom == null)
                    await Task.Delay(100);
                while (PhotonNetwork.CurrentRoom.PlayerCount < 2 + 1)
                    await Task.Delay(100);
            }
            else
            {
                while (!PhotonNetwork.IsConnectedAndReady)
                    await Task.Delay(100);
                while (PhotonNetwork.CurrentRoom == null)
                    await Task.Delay(100);
                while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                    await Task.Delay(100);
            }
        }
    }

    public void ProcessEvent(byte eventCode, object content, int senderId)
    {
        if (!(content is Hashtable parametersTable))
            return;
            
        switch (eventCode)
        {
            case PhotonEventsConstants.SyncMatch.SyncMatchConfigOnStartEventId:
                if (_matchEngine != null && _matchEngine.IsInited)
                    return;
                    
                SyncMatchParametersParser.Parameters syncMatchParameters = SyncMatchParametersParser.ParseParameters(parametersTable);
                InitAsClient(syncMatchParameters.ClientMatchParameters, syncMatchParameters.RolesAndUsers, syncMatchParameters.RandomSeed);
                break;

            default:
                _commandExecutorsAggregator.ProcessEvent(eventCode, parametersTable, senderId);
                break;
        }
    }

    private async Task InitAsServer()
    {
        DateTime now = DateTime.Now;
        int startTimeSum = now.Year + now.Month + now.Day + now.Hour + now.Minute;
        Randomizer.InitState(startTimeSum + 42);
        _connectionMaintainer.Init();
            
        PlayerHandParams playerHand = LoadPlayerHand();

        byte levelIdToPlay = (byte)Mathf.Clamp(PlayerPrefs.GetInt("Level", 1), 1, byte.MaxValue);

        MatchInitDataParameters matchParameters = new MatchInitDataParameters(
            levelIdToPlay,
            // mapModel.HexModels.ToArray(),
            // mapModel.PathDatas.ToArray(),
            // levelsConfig.Levels[levelIdToPlay].Waves, 
            //levelsConfig.Levels[levelIndexToPlay].CoinsCount,
            // levelsConfig.Levels[levelIdToPlay].EnergyStartCount,
            // MatchConfig.EnergyMaxCount,
            // levelsConfig.Levels[levelIdToPlay].EnergyRestoreDelay,
            // MatchConfig.EnergyRestoreValue,
            playerHand);
        
        _matchEngine = FindObjectOfType<TestMatchEngine>();
        _matchEngine.Init(matchParameters, _eventBus,
            _networkMatchStatus.CurrentProcessGameRoleReactiveProperty,
            _networkMatchStatus.CurrentProcessNetworkRoleReactiveProperty,
            _connectionMaintainer.IsConnectedReactiveProperty,
            RequestState,
            OnEndMatchHandler,
            OnQuitMatchHandler,
            _isMultiPlayerGame);

        // re-init seed, because server had many calls to random while creating MatchConfig
        int randomSeed = startTimeSum;
        Randomizer.InitState(startTimeSum);

        Hashtable roomProperties = new Hashtable
        {
            {PhotonEventsConstants.SyncMatch.MatchConfigRolesAndUsers, _networkMatchStatus.RolesAndUserIdsNetwork},
            {PhotonEventsConstants.SyncMatch.MatchConfigLevelIdParam, matchParameters.LevelId},
            // {PhotonEventsConstants.SyncMatch.MatchConfigWavesCount, (byte)matchParameters.Waves.Length},
            //{PhotonEventsConstants.SyncMatch.MatchConfigPathsCount, (byte)mapModel.PathDatas.Count},
            //{PhotonEventsConstants.SyncMatch.MatchConfigFieldTypesParam, matchParameters.GetHexesTypes()},
            //{PhotonEventsConstants.SyncMatch.MatchStartCoinsParam, matchParameters.CoinsCount},
            // {PhotonEventsConstants.SyncMatch.MatchStartEnergyParam, matchParameters.EnergyStartCount},
            // {PhotonEventsConstants.SyncMatch.MatchMaxEnergyParam, matchParameters.EnergyMaxCount},
            // {PhotonEventsConstants.SyncMatch.MatchRestoreEnergyDelay, matchParameters.EnergyRestoreDelay},
            // {PhotonEventsConstants.SyncMatch.MatchRestoreEnergyValue, matchParameters.EnergyRestoreValue},
            {PhotonEventsConstants.SyncMatch.MatchConfigHandTowersParam, matchParameters.PlayerHandParams.TowersNetwork},
            {PhotonEventsConstants.SyncMatch.RandomSeed, randomSeed}
        };

        // for (int waveIndex = 0; waveIndex < matchParameters.Waves.Length; waveIndex++)
        //     roomProperties[$"{PhotonEventsConstants.SyncMatch.MatchConfigWaveParam}{waveIndex}"] =
        //         matchParameters.Waves[waveIndex].ToNetwork();

        // var hexModels = mapModel.HexModels.ToArray();
        // Hashtable hexesModelTable = new Hashtable(hexModels.Length);
        // for (int hexIndex = 0; hexIndex < hexModels.Length; hexIndex++)
        //     hexesModelTable[$"{hexIndex}"] = hexModels[hexIndex].ToNetwork();
        // roomProperties[$"{PhotonEventsConstants.SyncMatch.MatchConfigHexFieldParam}"] = hexesModelTable;
        
        // var paths = mapModel.PathDatas.ToArray();
        // for (int pathIndex = 0; pathIndex < paths.Length; pathIndex++)
        //     roomProperties[$"{PhotonEventsConstants.SyncMatch.MatchConfigPathFieldParam}{pathIndex}"] =
        //         paths[pathIndex].ToNetwork();

        _eventBus.RaiseEvent(PhotonEventsConstants.SyncMatch.SyncMatchConfigOnStartEventId, roomProperties);
            
        // no server room in single player
        if (_isMultiPlayerGame)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = EmptyRoomTimeToLive;
        }

        CreateCommandExecutors();
    }

    private void InitAsClient(MatchInitDataParameters matchInitDataParameters, Dictionary<ProcessRoles, int> rolesAndUsers,
        int randomSeed)
    {
        CreateNetworkMatchStatus(rolesAndUsers);
        _connectionMaintainer.Init();
        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("Player id is " + playerId + ", role is " + _networkMatchStatus.CurrentProcessGameRoleReactiveProperty.Value);
        Debug.Log("Init client " + _networkMatchStatus.CurrentProcessGameRoleReactiveProperty.Value);
            
        // temporary stuff
        PlayerHandParams playerHand = LoadPlayerHand();
            
        Randomizer.InitState(randomSeed);
        _matchEngine = FindObjectOfType<TestMatchEngine>();
        _matchEngine.Init(matchInitDataParameters, _eventBus,
            _networkMatchStatus.CurrentProcessGameRoleReactiveProperty,
            _networkMatchStatus.CurrentProcessNetworkRoleReactiveProperty,
            _connectionMaintainer.IsConnectedReactiveProperty,
            RequestState,
            OnEndMatchHandler,
            OnQuitMatchHandler,
            _isMultiPlayerGame);
            
        // mark room 
        Hashtable roomProperties = new Hashtable
        {
            {PhotonEventsConstants.MatchStarted, true}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            
        CreateCommandExecutors();
    }

    private void ResumeGame()
    {
        SyncMatchParametersParser.Parameters syncMatchParameters = SyncMatchParametersParser.ParseParameters(PhotonNetwork.CurrentRoom.CustomProperties);
        // update roles and users
        Dictionary<ProcessRoles, int> rolesAndUsers = new Dictionary<ProcessRoles, int>(syncMatchParameters.RolesAndUsers.Count);
        int oldMasterId = syncMatchParameters.RolesAndUsers[ProcessRoles.Server];
        int newMasterId = PhotonNetwork.CurrentRoom.MasterClientId;

        foreach (var roleUserPair in syncMatchParameters.RolesAndUsers)
        {
            // update id for another player
            if (roleUserPair.Value == oldMasterId)
                rolesAndUsers.Add(roleUserPair.Key, newMasterId);
            // update id for current player
            else
                rolesAndUsers.Add(roleUserPair.Key, PhotonNetwork.LocalPlayer.ActorNumber);
        }
            
        InitAsClient(syncMatchParameters.ClientMatchParameters, rolesAndUsers, syncMatchParameters.RandomSeed);
        RequestState();
    }

    private void CreateEventBus()
    {
        if (_isMultiPlayerGame)
        {
            PhotonEventBus.Context eventBusContext = new PhotonEventBus.Context(this);
            _eventBus = new PhotonEventBus(eventBusContext);
        }
        else
        {
            LocalEventBus.Context eventBusContext =
                new LocalEventBus.Context(this, 1);
            _eventBus = new LocalEventBus(eventBusContext);
        }
    }

    private void CreateNetworkMatchStatus(Dictionary<ProcessRoles, int> rolesAndUserIds = null)
    {
        ProcessRolesDefiner.Context processRolesDefinerContext = new ProcessRolesDefiner.Context(_isMultiPlayerGame, HasAuthorityServer);
        _processRolesDefiner = new ProcessRolesDefiner(processRolesDefinerContext);
            
        if (rolesAndUserIds == null)
            rolesAndUserIds = _processRolesDefiner.GetRolesAndUsers();
            
        NetworkMatchStatus.Context networkMatchStatusContext = new NetworkMatchStatus.Context(_processRolesDefiner, rolesAndUserIds);
        _networkMatchStatus = new NetworkMatchStatus(networkMatchStatusContext);

        _connectionMaintainer.OnRoomReceivedPlayer += _networkMatchStatus.AddUser;
        _connectionMaintainer.OnRoomLostPlayer += _networkMatchStatus.RemoveUser;
    }
        
    private void CreateCommandExecutors()
    {
        CommandExecutorsAggregator.Context commandsAggregatorContext = new CommandExecutorsAggregator.Context(_matchEngine, _eventBus,
            _networkMatchStatus, _pingDamper.PingDamperFramesDeltaReactiveProperty);
        _commandExecutorsAggregator = new CommandExecutorsAggregator(commandsAggregatorContext);
    }

    private PlayerHandParams LoadPlayerHand()
    {
        TowerType[] towersInHand = new TowerType[TestMatchEngine.TowersInHandCount];

        for (int itemIndex = 0; itemIndex < TestMatchEngine.TowersInHandCount; itemIndex++)
        {
            // TODO: take from presaved hand
            towersInHand[itemIndex] = (TowerType)(itemIndex+1);
            //(TowerType)PlayerPrefs.GetInt($"CurrentHandItem{itemIndex}", (int)TowerType.Undefined);
        }

        return new PlayerHandParams(towersInHand);
    }

    private void SendPlayerHand()
    {
        int[] playerHandParameters = new int[TestMatchEngine.TowersInHandCount];
            
        for (int itemIndex = 0; itemIndex < TestMatchEngine.TowersInHandCount; itemIndex++)
        {
            int towerType = PlayerPrefs.GetInt($"CurrentHandItem{itemIndex}", (int)TowerType.Undefined);
            playerHandParameters[itemIndex] = towerType;
        }

        string key = $"Player{PhotonNetwork.LocalPlayer.ActorNumber}_hand";
        Hashtable roomProperties = new Hashtable(){ {key, playerHandParameters}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    private void OnEndMatchHandler()
    {
        _onEndMatch.OnNext(Unit.Default);
        
        if (_isMultiPlayerGame)
            PhotonNetwork.LeaveRoom();
        
        if (_connectionMaintainer != null)
        {
            _connectionMaintainer.OnRequestStateEvent -= RequestState;
            _connectionMaintainer.OnRollbackStateEvent -= RollbackState;
            _connectionMaintainer.OnRoomReceivedPlayer -= _networkMatchStatus.AddUser;
            _connectionMaintainer.OnRoomLostPlayer -= _networkMatchStatus.RemoveUser;
            _connectionMaintainer.Clear();
            Destroy(_connectionMaintainer);
        }

        _eventBus?.Dispose();
        _processRolesDefiner?.Dispose();
        _networkMatchStatus?.Dispose();
        _commandExecutorsAggregator?.Dispose();
        _pingDamper?.Dispose();
    }

    private void RequestState()
    {
        _eventBus.RaiseEvent(PhotonEventsConstants.SyncState.RequestEventId,
            new Hashtable{{PhotonEventsConstants.SyncState.TimeParam, _matchEngine.CurrentEngineFrameReactiveProperty.Value}});
    }

    private void RollbackState()
    {
        _matchEngine.RollbackState();
    }

    private void OnQuitMatchHandler()
    {
        Dispose();
        _onQuitMatch.OnNext(Unit.Default);
    }
    
    private void Dispose()
    {
        Destroy(_matchEngine);
        _isDisposed = true;
    }
}