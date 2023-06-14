using Match.Field;
using Match.State;
using Match.Wave;
using Tools;
using UniRx;

namespace Match
{
    public class MatchStateSaver : BaseDisposable
    {
        public struct Context
        {
            public FieldController Player1FieldController { get; }
            public FieldController Player2FieldController { get; }
            public WaveMobSpawnerCoordinator WaveMobSpawnerCoordinator { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }

            public Context(FieldController player1FieldController, FieldController player2FieldController,
                WaveMobSpawnerCoordinator waveMobSpawnerCoordinator,
                ReactiveCommand<float> waveStartedReactiveCommand,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty)
            {
                Player1FieldController = player1FieldController;
                Player2FieldController = player2FieldController;
                WaveMobSpawnerCoordinator = waveMobSpawnerCoordinator;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
            }
        }

        private readonly Context _context;
        private const int SaveDelayInEngineFrames = 50;
        private MatchState _lastMatchState; 

        public MatchStateSaver(Context context)
        {
            _context = context;

            // use savings at wave start to rollback to wave start
            //_context.WaveStartedReactiveCommand.Subscribe((waveStartDelay) => SaveMatchState());
            _context.CurrentEngineFrameReactiveProperty
                .Where(currentFrame => currentFrame % SaveDelayInEngineFrames == 0).Subscribe((x) => SaveMatchState());
        }

        private void SaveMatchState()
        {
            _lastMatchState = new MatchState(
                _context.Player1FieldController.GetPlayerState(),
                _context.Player2FieldController.GetPlayerState(),
                _context.WaveMobSpawnerCoordinator.GetWavesState(),
                Randomizer.CurrentSeed, Randomizer.RandomCallsCountReactiveProperty.Value);
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
    }
}