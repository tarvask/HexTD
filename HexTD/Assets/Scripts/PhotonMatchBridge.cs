using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Match;
using Match.Commands;
using Match.EventBus;
using Match.Field.Tower;
using Photon.PhotonRealtime.Code;
using Photon.PhotonUnityNetworking.Code;
using Services;
using Services.PhotonRelated;
using Tools;
using UnityEngine;

public class PhotonMatchBridge : BaseMonoBehaviour
{
    [SerializeField] private bool isMultiPlayerGame;
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

    private bool IsServer => !isMultiPlayerGame || PhotonNetwork.IsMasterClient;
        
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
            
        if (isMultiPlayerGame)
            SendPlayerHand();
            
        _players = new List<Player>(PhotonNetwork.PlayerList);
            
        if (IsServer)
        {
            await Task.Delay(3000);
            CreateNetworkMatchStatus();
            CreateEventBus();
            InitAsServer();
        }
        else
            CreateEventBus();
            
        if (isMultiPlayerGame && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PhotonEventsConstants.MatchStarted))
            ResumeGame();
    }

    private async Task WaitForFullRoom()
    {
        if (isMultiPlayerGame)
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

    private void InitAsServer()
    {
        DateTime now = DateTime.Now;
        int startTimeSum = now.Year + now.Month + now.Day + now.Hour + now.Minute;
        Randomizer.InitState(startTimeSum + 42);
        _connectionMaintainer.Init();
            
        PlayerHandParams playerHand = LoadPlayerHand();

        int levelIndexToPlay = Mathf.Clamp(PlayerPrefs.GetInt("Level", 0), 0, levelsConfig.Levels.Length - 1);
            
        MatchParameters matchParameters = new MatchParameters(levelsConfig.Levels[levelIndexToPlay], playerHand);
        _matchEngine = FindObjectOfType<TestMatchEngine>();
        _matchEngine.Init(matchParameters, _eventBus,
            _networkMatchStatus.CurrentProcessGameRoleReactiveProperty,
            _networkMatchStatus.CurrentProcessNetworkRoleReactiveProperty,
            _connectionMaintainer.IsConnectedReactiveProperty,
            LeaveRoom,
            Dispose,
            isMultiPlayerGame); //ProcessRoles.Player1

        // re-init seed, because server had many calls to random while creating MatchConfig
        int randomSeed = startTimeSum;
        Randomizer.InitState(startTimeSum);

        Hashtable roomProperties = new Hashtable
        {
            {PhotonEventsConstants.SyncMatch.MatchConfigRolesAndUsers, _networkMatchStatus.RolesAndUserIdsNetwork},
            {PhotonEventsConstants.SyncMatch.MatchConfigWavesCount, (byte)matchParameters.Waves.Length},
            {PhotonEventsConstants.SyncMatch.MatchConfigFieldParam, matchParameters.CellsNetwork},
            {PhotonEventsConstants.SyncMatch.MatchConfigBlockersParam, matchParameters.BlockersNetwork},
            {PhotonEventsConstants.SyncMatch.MatchStartSilverCoinsParam, matchParameters.SilverCoinsCount},
            {PhotonEventsConstants.SyncMatch.MatchStartSpellsAndCounts, matchParameters.SpellsWithCountsNetwork},
            {PhotonEventsConstants.SyncMatch.MatchConfigHandTowersParam, matchParameters.HandParams.TowersNetwork},
            {PhotonEventsConstants.SyncMatch.RandomSeed, randomSeed}
        };

        for (int waveIndex = 0; waveIndex < matchParameters.Waves.Length; waveIndex++)
            roomProperties[$"{PhotonEventsConstants.SyncMatch.MatchConfigWaveParam}{waveIndex}"] =
                matchParameters.Waves[waveIndex].ToNetwork();

        _eventBus.RaiseEvent(PhotonEventsConstants.SyncMatch.SyncMatchConfigOnStartEventId, roomProperties);
            
        // no server room in single player
        if (isMultiPlayerGame)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = EmptyRoomTimeToLive;
        }

        CreateCommandExecutors();
    }

    private void InitAsClient(MatchParameters matchShortParameters, Dictionary<ProcessRoles, int> rolesAndUsers,
        int randomSeed)
    {
        CreateNetworkMatchStatus(rolesAndUsers);
        _connectionMaintainer.Init();
        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("Player id is " + playerId + ", role is " + _networkMatchStatus.CurrentProcessGameRoleReactiveProperty.Value);
        Debug.Log("Init client " + _networkMatchStatus.CurrentProcessGameRoleReactiveProperty.Value);
            
        // temporary stuff
        PlayerHandParams playerHand = LoadPlayerHand();
        matchShortParameters.ChangeHand(playerHand);
            
        Randomizer.InitState(randomSeed);
        _matchEngine = FindObjectOfType<TestMatchEngine>();
        _matchEngine.Init(matchShortParameters, _eventBus,
            _networkMatchStatus.CurrentProcessGameRoleReactiveProperty,
            _networkMatchStatus.CurrentProcessNetworkRoleReactiveProperty,
            _connectionMaintainer.IsConnectedReactiveProperty,
            LeaveRoom,
            Dispose,
            isMultiPlayerGame);
            
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
        if (isMultiPlayerGame)
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
        ProcessRolesDefiner.Context processRolesDefinerContext = new ProcessRolesDefiner.Context(isMultiPlayerGame, HasAuthorityServer);
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
        TowerType[] towersInHand = new TowerType[8];
            
        for (int itemIndex = 0; itemIndex < 8; itemIndex++)
            towersInHand[itemIndex] = (TowerType)PlayerPrefs.GetInt($"CurrentHandItem{itemIndex}", (int)TowerType.Undefined);
            
        return new PlayerHandParams(towersInHand);;
    }

    private void SendPlayerHand()
    {
        int[] playerHandParameters = new int[8];
            
        for (int itemIndex = 0; itemIndex < 8; itemIndex++)
        {
            int towerType = PlayerPrefs.GetInt($"CurrentHandItem{itemIndex}", (int)TowerType.Undefined);
            playerHandParameters[itemIndex] = towerType;
        }

        string key = $"Player{PhotonNetwork.LocalPlayer.ActorNumber}_hand";
        Hashtable roomProperties = new Hashtable(){ {key, playerHandParameters}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.Disconnect();
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

    private void Dispose()
    {
        Destroy(_matchEngine);

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

        _isDisposed = true;
    }
}