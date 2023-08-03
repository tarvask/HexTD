using ExitGames.Client.Photon;
using Match.Field.State;
using Match.Wave;

namespace Match.State
{
    public readonly struct MatchState
    {
        private readonly PlayerState _player1State;
        private readonly PlayerState _player2State;
        private readonly WavesState _wavesState;
        private readonly int _randomSeed;
        private readonly int _randomCounter;

        public PlayerState Player1State => _player1State;
        public PlayerState Player2State => _player2State;
        public WavesState WavesState => _wavesState;
        public int RandomSeed => _randomSeed;
        public int RandomCounter => _randomCounter;

        public MatchState(in PlayerState player1State, in PlayerState player2State,
            in WavesState wavesState,
            int randomSeed, int randomCounter)
        {
            _player1State = player1State;
            _player2State = player2State;
            
            _wavesState = wavesState;
            _randomSeed = randomSeed;
            _randomCounter = randomCounter;
        }
        
        public static MatchState FromHashtable(Hashtable matchStateHashtable)
        {
            PlayerState player1State = PlayerState.FromHashtable((Hashtable) matchStateHashtable[PhotonEventsConstants.SyncState.MatchState.Player1StateParam]);
            PlayerState player2State = PlayerState.FromHashtable((Hashtable) matchStateHashtable[PhotonEventsConstants.SyncState.MatchState.Player2StateParam]);
            
            WavesState wavesState = WavesState.FromHashtable((Hashtable) matchStateHashtable[PhotonEventsConstants.SyncState.MatchState.WaveStateParam]);
            
            int randomSeed = (int)matchStateHashtable[PhotonEventsConstants.SyncState.MatchState.RandomSeedParam];
            int randomCounter = (int)matchStateHashtable[PhotonEventsConstants.SyncState.MatchState.RandomCounterParam];

            return new MatchState(in player1State, in player2State, wavesState, randomSeed, randomCounter);
        }

        public static Hashtable ToHashtable(in MatchState matchState)
        {
            return new Hashtable
            {
                {PhotonEventsConstants.SyncState.MatchState.Player1StateParam, PlayerState.ToHashtable(matchState.Player1State)},
                {PhotonEventsConstants.SyncState.MatchState.Player2StateParam, PlayerState.ToHashtable(matchState.Player2State)},
                {PhotonEventsConstants.SyncState.MatchState.WaveStateParam, WavesState.ToHashtable(matchState.WavesState)},
                {PhotonEventsConstants.SyncState.MatchState.RandomSeedParam, matchState.RandomSeed},
                {PhotonEventsConstants.SyncState.MatchState.RandomCounterParam, matchState.RandomCounter},
            };
        }
    }
}