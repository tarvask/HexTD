using System.Collections.Generic;
using Match.Field.Castle;
using Match.Field.Mob;
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
            public Camera MainCamera { get; }
            public Canvas Canvas { get; }
            public MobConfigRetriever MobConfigRetriever { get; }
            public WaveParams[] Waves { get; }
            
            public ReactiveCommand<HealthInfo> EnemyCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<HealthInfo> OurCastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand<float> WaveStartedReactiveCommand { get; }
            public ReactiveCommand<float> BetweenWavesPlanningStartedReactiveCommand { get; }
            public ReactiveCommand<float> ArtifactChoosingStartedReactiveCommand { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurGoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurGoldenCoinsIncomeChangedReactiveCommand { get; }
            public ReactiveCommand<int> OurCrystalsCountChangedReactiveCommand { get; }

            public Context(MatchInfoPanelView view, Camera mainCamera, Canvas canvas, MobConfigRetriever mobConfigRetriever, WaveParams[] waves,
                
                ReactiveCommand<HealthInfo> enemyCastleHealthChangedReactiveCommand,
                ReactiveCommand<HealthInfo> ourCastleHealthChangedReactiveCommand,
                ReactiveCommand<float> waveStartedReactiveCommand,
                ReactiveCommand<float> betweenWavesPlanningStartedReactiveCommand,
                ReactiveCommand<float> artifactChoosingStartedReactiveCommand,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand<int> ourGoldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> ourGoldenCoinsIncomeChangedReactiveCommand,
                ReactiveCommand<int> ourCrystalsCountChangedReactiveCommand)
            {
                View = view;
                MainCamera = mainCamera;
                Canvas = canvas;
                MobConfigRetriever = mobConfigRetriever;
                Waves = waves;

                EnemyCastleHealthChangedReactiveCommand = enemyCastleHealthChangedReactiveCommand;
                OurCastleHealthChangedReactiveCommand = ourCastleHealthChangedReactiveCommand;
                WaveStartedReactiveCommand = waveStartedReactiveCommand;
                BetweenWavesPlanningStartedReactiveCommand = betweenWavesPlanningStartedReactiveCommand;
                ArtifactChoosingStartedReactiveCommand = artifactChoosingStartedReactiveCommand;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                OurGoldenCoinsCountChangedReactiveCommand = ourGoldenCoinsCountChangedReactiveCommand;
                OurGoldenCoinsIncomeChangedReactiveCommand = ourGoldenCoinsIncomeChangedReactiveCommand;
                OurCrystalsCountChangedReactiveCommand = ourCrystalsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MatchInfoRoundStartInfoPanelController _roundStartInfoPanelController;
        private readonly Dictionary<byte, byte> _enemyReinforcementsItems;
        private int _waveNumber;
        private float _currentTimer;
        private int _currentTimerInt;
        private bool _waveSpawningInProcess;

        public int MatchInfoPanelTopShift =>
            Mathf.RoundToInt(((RectTransform)_context.View.transform.parent).rect.height * 0.5f 
                             - (_context.View.MiddlePanelRect.anchoredPosition.y + _context.View.MiddlePanelRect.rect.height * 0.5f));

        public MatchInfoPanelController(Context context)
        {
            _context = context;
            
            MatchInfoRoundStartInfoPanelController.Context roundStartInfoPanelControllerContext =
                new MatchInfoRoundStartInfoPanelController.Context(_context.View.RoundStartInfoPanel);
            _roundStartInfoPanelController = AddDisposable(new MatchInfoRoundStartInfoPanelController(roundStartInfoPanelControllerContext));
            _enemyReinforcementsItems = new Dictionary<byte, byte>();
            
            _context.EnemyCastleHealthChangedReactiveCommand.Subscribe(OnEnemyCastleHealthChangedEventHandler);
            _context.OurCastleHealthChangedReactiveCommand.Subscribe(OnOurCastleHealthChangedEventHandler);
            _context.WaveNumberChangedReactiveCommand.Subscribe(OnWaveNumberChangedEventHandler);
            _context.ArtifactChoosingStartedReactiveCommand.Subscribe(OnArtifactChoosingStartedEventHandler);
            _context.BetweenWavesPlanningStartedReactiveCommand.Subscribe(OnBetweenWavesPlanningStartedEventHandler);
            _context.WaveStartedReactiveCommand.Subscribe(OnWaveStartedEventHandler);
            _context.OurGoldenCoinsCountChangedReactiveCommand.Subscribe(OnOurGoldenCoinsCountChangedEventHandler);
            _context.OurGoldenCoinsIncomeChangedReactiveCommand.Subscribe(OnOurGoldenCoinsIncomeChangedEventHandler);
            _context.OurCrystalsCountChangedReactiveCommand.Subscribe(OnOurCrystalsCountChangedEventHandler);

            // show contents of Wave 1
            RefreshReinforcementsInfo(0);
        }

        public void AddReinforcementsItem(byte mobId)
        {
            if (_enemyReinforcementsItems.ContainsKey(mobId))
                _enemyReinforcementsItems[mobId]++;
            else
            {
                _enemyReinforcementsItems.Add(mobId, 1);
                _context.View.ReinforcementsItems[_enemyReinforcementsItems.Count - 1].sprite =
                    _context.MobConfigRetriever.GetMobById(mobId).Icon;
            }
        }
        
        public void RemoveReinforcementsItem(byte mobId)
        {
            if (_enemyReinforcementsItems.ContainsKey(mobId))
            {
                _enemyReinforcementsItems[mobId]--;

                if (_enemyReinforcementsItems[mobId] == 0)
                {
                    _enemyReinforcementsItems.Remove(mobId);
                    _context.View.ReinforcementsItems[_enemyReinforcementsItems.Count - 1].sprite = null;
                }
            }
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

        private void OnArtifactChoosingStartedEventHandler(float artifactChoosingDuration)
        {
            _currentTimer = artifactChoosingDuration + _context.Waves[Mathf.Min(_waveNumber - 1, _context.Waves.Length - 1)].PauseBeforeWave;
            _context.View.CurrentRoundWithTimer.gameObject.SetActive(false);
            _context.View.NextRoundStartWithTimer.gameObject.SetActive(true);
        }

        private void OnWaveStartedEventHandler(float waveInfoDuration)
        {
            _waveSpawningInProcess = true;
            _currentTimer = _context.Waves[Mathf.Min(_waveNumber, _context.Waves.Length - 1)].Duration;
            _context.View.CurrentRoundWithTimer.gameObject.SetActive(true);
            _context.View.NextRoundStartWithTimer.gameObject.SetActive(false);
            
            _roundStartInfoPanelController.Show(_waveNumber, _context.View.WaveElements, _context.View.ReinforcementsItems,
                waveInfoDuration);
            RefreshReinforcementsInfo(_waveNumber);
        }

        private void OnOurGoldenCoinsCountChangedEventHandler(int newGoldenCoinsCount)
        {
            _context.View.OurGoldCoinsCountText.text = $"{newGoldenCoinsCount}";
        }
        
        private void OnOurGoldenCoinsIncomeChangedEventHandler(int newIncomeSize)
        {
            _context.View.OurGoldCoinsIncomeText.text = $"{newIncomeSize}";
        }

        private void OnOurCrystalsCountChangedEventHandler(int newCrystalsCount)
        {
            _context.View.OurCrystalsCountText.text = $"{newCrystalsCount}";
        }

        private void RefreshReinforcementsInfo(int waveNumber)
        {
            int nextWaveIndex = Mathf.Clamp(waveNumber, 0, _context.Waves.Length - 1);
            // drop reinforcements for next round
            _enemyReinforcementsItems.Clear();

            for (int waveElementIndex = 0; waveElementIndex < _context.View.WaveElements.Length; waveElementIndex++)
            {
                if (waveElementIndex < _context.Waves[nextWaveIndex].Elements.Length)
                {
                    _context.View.WaveElements[waveElementIndex].sprite =
                        _context.MobConfigRetriever
                            .GetMobById(_context.Waves[nextWaveIndex].Elements[waveElementIndex].MobId).Icon;
                }
                else
                    _context.View.WaveElements[waveElementIndex].sprite = null;
            }
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