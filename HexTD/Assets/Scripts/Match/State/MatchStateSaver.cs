using BuffLogic;
using ExitGames.Client.Photon;
using Match.Field;
using Match.Field.Hand;
using Match.Serialization;
using Match.State.CheckSum;
using Match.Wave;
using Tools;
using UniRx;

namespace Match.State
{
    public class MatchStateSaver : BaseDisposable
    {
        public struct Context
        {
            public FieldController Player1FieldController { get; }
            public FieldController Player2FieldController { get; }
            public MatchStateCheckSumComputerController CheckSumComputerController { get; }
            public WaveMobSpawnerCoordinator WaveMobSpawnerCoordinator { get; }
            public BuffManager BuffManager { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public ReactiveCommand<MatchStateCheckSum> MatchStateCheckSumComputedReactiveCommand { get; }
            public IReadOnlyReactiveProperty<bool> IsMatchRunningReactiveProperty { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }

            public Context(FieldController player1FieldController, FieldController player2FieldController,
                MatchStateCheckSumComputerController checkSumComputerController,
                WaveMobSpawnerCoordinator waveMobSpawnerCoordinator,
                BuffManager buffManager,
                ReactiveCommand<float> waveStartedReactiveCommand,
                ReactiveCommand<MatchStateCheckSum> matchStateCheckSumComputedReactiveCommand,
                IReadOnlyReactiveProperty<bool> isMatchRunningReactiveProperty,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty)
            {
                Player1FieldController = player1FieldController;
                Player2FieldController = player2FieldController;
                CheckSumComputerController = checkSumComputerController;
                WaveMobSpawnerCoordinator = waveMobSpawnerCoordinator;
                BuffManager = buffManager;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                MatchStateCheckSumComputedReactiveCommand = matchStateCheckSumComputedReactiveCommand;
                IsMatchRunningReactiveProperty = isMatchRunningReactiveProperty;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
            }
        }

        private const int SaveDelayInEngineFrames = 50;
        
        private readonly Context _context;
        private MatchState _lastMatchState;
        private bool _hasSavedStateOnMatchEnd;

        public MatchStateSaver(Context context)
        {
            _context = context;

            // use savings at wave start to rollback to wave start
            //_context.WaveStartedReactiveCommand.Subscribe((waveStartDelay) => SaveMatchState());
            _context.CurrentEngineFrameReactiveProperty.Subscribe(OnCurrentEngineFrameChanged);
        }

        private void OnCurrentEngineFrameChanged(int currentEngineFrame)
        {
            if (currentEngineFrame % SaveDelayInEngineFrames == 0 && !_hasSavedStateOnMatchEnd)
            {
                SaveMatchState();

                if (!_context.IsMatchRunningReactiveProperty.Value)
                    _hasSavedStateOnMatchEnd = true;
            }
        }

        private void SaveMatchState()
        {
            _lastMatchState = new MatchState(
                _context.Player1FieldController.GetPlayerState(),
                _context.Player2FieldController.GetPlayerState(),
                _context.WaveMobSpawnerCoordinator.GetWavesState(),
                _context.BuffManager.ToNetwork(),
                Randomizer.CurrentSeed, Randomizer.RandomCallsCountReactiveProperty.Value);
            
            _context.CheckSumComputerController.UpdateCheckSumHistory(_context.CurrentEngineFrameReactiveProperty.Value, _lastMatchState);
            _context.MatchStateCheckSumComputedReactiveCommand.Execute(_context.CheckSumComputerController.LastCheckSum);
        }
        
        public ref MatchState GetCurrentMatchState()
        {
            SaveMatchState();
            return ref _lastMatchState;
        }

        public ref MatchState GetLastSavedMatchState()
        {
            return ref _lastMatchState;
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable hashtable = new Hashtable();
            
            SerializerToNetwork.AddToHashTable(_context.Player1FieldController, hashtable, "1");
            SerializerToNetwork.AddToHashTable(_context.Player2FieldController, hashtable, "2");
            SerializerToNetwork.AddToHashTable(_context.WaveMobSpawnerCoordinator, hashtable, PhotonEventsConstants.SyncState.MatchState.WaveStateParam);
            
            hashtable.Add(PhotonEventsConstants.SyncState.MatchState.RandomSeedParam, Randomizer.CurrentSeed);
            hashtable.Add(PhotonEventsConstants.SyncState.MatchState.RandomCounterParam, Randomizer.RandomCallsCountReactiveProperty.Value);

            SerializerToNetwork.AddToHashTable(_context.BuffManager, hashtable, PhotonEventsConstants.SyncState.MatchState.BuffsParam);
            
            return hashtable;
        }
    }
}