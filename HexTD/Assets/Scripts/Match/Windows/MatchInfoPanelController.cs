using Match.Field.Castle;
using Match.Wave;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match.Windows
{
    public class MatchInfoPanelController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public MatchInfoPanelView View { get; }
            public Canvas Canvas { get; }
            public readonly bool IsMultiPlayerGame { get; }
            public WaveParametersStrict[] Waves { get; }
            
            public ReactiveCommand<HealthInfo> EnemyCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<HealthInfo> OurCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public ReactiveCommand<float> BetweenWavesPlanningStartedReactiveCommand { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurGoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurCrystalsCountChangedReactiveCommand { get; }

            public Context(MatchInfoPanelView view, Canvas canvas, 
                bool isMultiPlayerGame,
                WaveParametersStrict[] waves,
                ReactiveCommand<HealthInfo> enemyCastleHealthChangedReactiveCommand,
                ReactiveCommand<HealthInfo> ourCastleHealthChangedReactiveCommand,
                ReactiveCommand<float> waveStartedReactiveCommand,
                ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand<int> ourGoldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> ourCrystalsCountChangedReactiveCommand)
            {
                View = view;
                Canvas = canvas;
                IsMultiPlayerGame = isMultiPlayerGame;
                Waves = waves;

                EnemyCastleHealthChangedReactiveCommand = enemyCastleHealthChangedReactiveCommand;
                OurCastleHealthChangedReactiveCommand = ourCastleHealthChangedReactiveCommand;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                BetweenWavesPlanningStartedReactiveCommand = betweenWavesPlanningStartedReactiveCommand;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                OurGoldenCoinsCountChangedReactiveCommand = ourGoldenCoinsCountChangedReactiveCommand;
                OurCrystalsCountChangedReactiveCommand = ourCrystalsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MatchInfoRoundStartInfoPanelController _roundStartInfoPanelController;
        private int _waveNumber;
        private float _currentTimer;
        private int _currentTimerInt;
        private bool _waveSpawningInProcess;

        public MatchInfoPanelController(Context context)
        {
            _context = context;
            
            _context.View.EnemyFieldImage.gameObject.SetActive(_context.IsMultiPlayerGame);
            
            MatchInfoRoundStartInfoPanelController.Context roundStartInfoPanelControllerContext =
                new MatchInfoRoundStartInfoPanelController.Context(_context.View.RoundStartInfoPanel);
            _roundStartInfoPanelController = AddDisposable(new MatchInfoRoundStartInfoPanelController(roundStartInfoPanelControllerContext));

            _context.EnemyCastleHealthChangedReactiveCommand.Subscribe(OnEnemyCastleHealthChangedEventHandler);
            _context.OurCastleHealthChangedReactiveCommand.Subscribe(OnOurCastleHealthChangedEventHandler);
            _context.WaveNumberChangedReactiveCommand.Subscribe(OnWaveNumberChangedEventHandler);
            _context.BetweenWavesPlanningStartedReactiveCommand.Subscribe(OnBetweenWavesPlanningStartedEventHandler);
            _context.WaveStartedReactiveCommand.Subscribe(OnWaveStartedEventHandler);
            _context.OurGoldenCoinsCountChangedReactiveCommand.Subscribe(OnOurGoldenCoinsCountChangedEventHandler);
            _context.OurCrystalsCountChangedReactiveCommand.Subscribe(OnOurCrystalsCountChangedEventHandler);
        }

        private void OnEnemyCastleHealthChangedEventHandler(HealthInfo castleHealthInfo)
        {
            _context.View.EnemyCastleHealthBar.fillAmount = (float)castleHealthInfo.CurrentHealth / castleHealthInfo.MaxHealth;
            _context.View.EnemyCastleHealthText.text = $"{castleHealthInfo.CurrentHealth} / {castleHealthInfo.MaxHealth}";
        }
        
        private void OnOurCastleHealthChangedEventHandler(HealthInfo castleHealthInfo)
        {
            _context.View.OurCastleHealthBar.fillAmount = (float)castleHealthInfo.CurrentHealth / castleHealthInfo.MaxHealth;
            _context.View.OurCastleHealthText.text = $"{castleHealthInfo.CurrentHealth} / {castleHealthInfo.MaxHealth}";
        }

        private void OnWaveNumberChangedEventHandler(int waveNumber)
        {
            _waveNumber = waveNumber;
            _waveSpawningInProcess = false;
        }

        private void OnBetweenWavesPlanningStartedEventHandler(float planningDuration)
        {
            _currentTimer = _context.Waves[Mathf.Min(_waveNumber - 1, _context.Waves.Length - 1)].PauseBeforeWave;
            _context.View.CurrentRoundWithTimer.gameObject.SetActive(false);
            _context.View.NextRoundStartWithTimer.gameObject.SetActive(true);
        }

        private void OnWaveStartedEventHandler(float waveInfoDuration)
        {
            _waveSpawningInProcess = true;
            _currentTimer = _context.Waves[Mathf.Min(_waveNumber, _context.Waves.Length - 1)].Duration;
            _context.View.CurrentRoundWithTimer.gameObject.SetActive(true);
            _context.View.NextRoundStartWithTimer.gameObject.SetActive(false);
            
            _roundStartInfoPanelController.Show(_waveNumber, waveInfoDuration);
        }

        private void OnOurGoldenCoinsCountChangedEventHandler(int newGoldenCoinsCount)
        {
            _context.View.OurGoldCoinsCountText.text = $"{newGoldenCoinsCount}";
        }

        private void OnOurCrystalsCountChangedEventHandler(int newCrystalsCount)
        {
            _context.View.OurCrystalsCountText.text = $"{newCrystalsCount}";
        }

        public void OuterLogicUpdate(float frameLength)
        {
            if (_roundStartInfoPanelController.IsShown)
                _roundStartInfoPanelController.OuterLogicUpdate(frameLength);
                
            if (_currentTimer <= 0)
                return;
            
            _currentTimer -= frameLength;
            int flooredTimer = Mathf.CeilToInt(_currentTimer);

            if (_currentTimerInt != flooredTimer)
            {
                _currentTimerInt = flooredTimer;
                    
                if (_waveSpawningInProcess)
                    _context.View.CurrentRoundWithTimer.text = $"{_waveNumber} Round: {_currentTimerInt}";
                else
                    _context.View.NextRoundStartWithTimer.text = $"Start in: {_currentTimerInt}";
            }
        }
    }
}