using System.Collections.Generic;
using HexSystem;
using MapEditor;
using Match.Commands;
using Match.Field.Castle;
using Match.Field.Currency;
using Match.Field.Mob;
using Match.Field.Services;
using Match.Field.Shooting;
using Match.Field.State;
using Match.Field.Tower;
using Match.Windows;
using Match.Windows.Tower;
using Services;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldController : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public FieldView FieldView { get; }
            public MatchShortParameters MatchShortParameters { get; }
            public FieldConfig FieldConfig { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public bool NeedsInput { get; }
            public MatchCommands MatchCommands { get; }
            public MatchInfoPanelController MatchInfoPanelController { get; }
            public TowerSelectionWindowController TowerSelectionWindowController { get; }
            public TowerManipulationWindowController TowerManipulationWindowController { get; }
            public TowerInfoWindowController TowerInfoWindowController { get; }
            public MobInfoWindowController MobInfoWindowController { get; }
            
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }
            public ReactiveCommand<Hex2d> ClickReactiveCommand { get; }
            public ReactiveCommand<PlayerState> StateSyncedReactiveCommand { get; }
            public ReactiveCommand<MobConfig> SpawnMobReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnField { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand WaveEndedReactiveCommand { get; }
            public ReactiveCommand<float> ArtifactChoosingStartedReactiveCommand { get; }
            public ReactiveCommand<HealthInfo> CastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }
            public ReactiveCommand<int> GoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> GoldenCoinsIncomeChangedReactiveCommand { get; }
            public ReactiveCommand<int> CrystalsCountChangedReactiveCommand { get; }

            public Context(FieldView fieldView,
                MatchShortParameters matchShortParameters, FieldConfig fieldConfig,
                ConfigsRetriever configsRetriever,

                bool needsInput,
                MatchInfoPanelController matchInfoPanelController,
                TowerSelectionWindowController towerSelectionWindowController,
                TowerManipulationWindowController towerManipulationWindowController,
                TowerInfoWindowController towerInfoWindowController,
                MobInfoWindowController mobInfoWindowController,
                
                MatchCommands matchCommands,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty,
                ReactiveCommand<Hex2d> clickReactiveCommand,
                ReactiveCommand<PlayerState> stateSyncedReactiveCommand,
                ReactiveCommand<MobConfig> spawnMobReactiveCommand,
                ReactiveProperty<bool> hasMobsOnField,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand waveEndedReactiveCommand,
                ReactiveCommand<float> artifactChoosingStartedReactiveCommand,
                ReactiveCommand<HealthInfo> castleHealthChangedReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand,
                ReactiveCommand<int> goldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> goldenCoinsIncomeChangedReactiveCommand,
                ReactiveCommand<int> crystalsCountChangedReactiveCommand)
            {
                FieldView = fieldView;
                MatchShortParameters = matchShortParameters;
                FieldConfig = fieldConfig;
                ConfigsRetriever = configsRetriever;

                NeedsInput = needsInput;
                MatchCommands = matchCommands;
                MatchInfoPanelController = matchInfoPanelController;
                TowerSelectionWindowController = towerSelectionWindowController;
                TowerManipulationWindowController = towerManipulationWindowController;
                TowerInfoWindowController = towerInfoWindowController;
                MobInfoWindowController = mobInfoWindowController;
                
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
                ClickReactiveCommand = clickReactiveCommand;
                StateSyncedReactiveCommand = stateSyncedReactiveCommand;
                SpawnMobReactiveCommand = spawnMobReactiveCommand;
                HasMobsOnField = hasMobsOnField;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                WaveEndedReactiveCommand = waveEndedReactiveCommand;
                ArtifactChoosingStartedReactiveCommand = artifactChoosingStartedReactiveCommand;
                CastleHealthChangedReactiveCommand = castleHealthChangedReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
                GoldenCoinsCountChangedReactiveCommand = goldenCoinsCountChangedReactiveCommand;
                GoldenCoinsIncomeChangedReactiveCommand = goldenCoinsIncomeChangedReactiveCommand;
                CrystalsCountChangedReactiveCommand = crystalsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly FieldModel _model;

        private readonly FieldFactory _factory;
        private readonly FieldMobSpawner _fieldMobSpawner;
        private readonly MobsManager _mobsManager;
        private readonly FieldClicksHandler _clicksHandler;
        private readonly FieldClicksDistributor _clicksDistributor;
        private readonly FieldConstructionProcessController _constructionProcessController;
        private readonly ShootingController _shootingController;
        private readonly CurrencyController _currencyController;
        private readonly PlayerStateLoader _stateLoader;
        
        public const float MoveLerpCoeff = 0.7f;

        public FieldModel FieldModel => _model;
        
        public FieldController(Context context)
        {
            _context = context;
            
            ReactiveCommand<int> castleAttackedByMobReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<MobController> removeMobReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<MobController> mobSpawnedReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<int> crystalCollectedReactiveCommand = AddDisposable(new ReactiveCommand<int>());

            Layout layout = new Layout(_context.FieldConfig.HexSettingsConfig.HexSize,
                Vector3.zero, _context.FieldConfig.HexSettingsConfig.IsFlat);
            HexagonalFieldController hexagonalFieldController = new HexagonalFieldController(layout,
                _context.MatchShortParameters.Hexes);
            TowersManager towersManager = new TowersManager(hexagonalFieldController.HexGridSize);
            
            FieldFactory.Context factoryContext = new FieldFactory.Context(
                _context.FieldView.transform, 
                hexagonalFieldController,
                _context.FieldConfig.CastleHealth, _context.FieldConfig.TowerRemovingDuration,
                castleAttackedByMobReactiveCommand,
                _context.CastleDestroyedReactiveCommand,
                removeMobReactiveCommand);
            _factory = AddDisposable(new FieldFactory(factoryContext));
            
            FieldModel.Context fieldModelContext = new FieldModel.Context(hexagonalFieldController,
                towersManager,
                _factory,
                removeMobReactiveCommand,
                mobSpawnedReactiveCommand,
                _context.HasMobsOnField);
            _model = AddDisposable(new FieldModel(fieldModelContext));

            FieldMobSpawner.Context mobSpawnerContext = new FieldMobSpawner.Context(_model, _factory, 
                _context.SpawnMobReactiveCommand);
            _fieldMobSpawner = AddDisposable(new FieldMobSpawner(mobSpawnerContext));
            
            MobsManager.Context mobMoverContext = new MobsManager.Context(_model, castleAttackedByMobReactiveCommand);
            _mobsManager = AddDisposable(new MobsManager(mobMoverContext));

            // we only need input for player field
            if (_context.NeedsInput)
            {
                FieldClicksHandler.Context clicksHandlerContext = new FieldClicksHandler.Context(
                    new Vector2(-0.5f, -0.5f), 1,
                    _context.ClickReactiveCommand);
                _clicksHandler = AddDisposable(new FieldClicksHandler(clicksHandlerContext));
            }
            
            // construction
            FieldConstructionProcessController.Context constructionProcessControllerContext =
                new FieldConstructionProcessController.Context(_model, _factory);
            _constructionProcessController = AddDisposable(new FieldConstructionProcessController(constructionProcessControllerContext));
            
            // shooting
            ShootingController.Context shootingControllerContext = new ShootingController.Context(_model, _factory);
            _shootingController = AddDisposable(new ShootingController(shootingControllerContext));
            
            // currency
            CurrencyController.Context currencyControllerContext = new CurrencyController.Context(_context.MatchShortParameters.SilverCoinsCount,
                5, removeMobReactiveCommand, crystalCollectedReactiveCommand);
            _currencyController = AddDisposable(new CurrencyController(currencyControllerContext));

            // area buffs
            //FieldTowersAreaBuffsManager.Context towersAreaBuffsManagerContext = new FieldTowersAreaBuffsManager.Context(
            //    _model, towerBuiltReactiveCommand, towerPreUpgradedReactiveCommand, towerUpgradedReactiveCommand, towerRemovedReactiveCommand);
            //_towersAreaBuffsManager = AddDisposable(new FieldTowersAreaBuffsManager(towersAreaBuffsManagerContext));

            // clicks distribution
            FieldClicksDistributor.Context clicksDistributorContext =
                new FieldClicksDistributor.Context(_model, _clicksHandler, _context.ConfigsRetriever,
                    _constructionProcessController,
                    _shootingController, _currencyController, _context.MatchCommands,
                    _context.TowerSelectionWindowController,
                    _context.TowerManipulationWindowController,
                    _context.TowerInfoWindowController);
            _clicksDistributor = AddDisposable(new FieldClicksDistributor(clicksDistributorContext));
            
            PlayerStateLoader.Context stateLoaderContext = new PlayerStateLoader.Context(_model, _factory,
                _context.ConfigsRetriever,
                _currencyController);
            _stateLoader = AddDisposable(new PlayerStateLoader(stateLoaderContext));

            CreateCells();

            _context.StateSyncedReactiveCommand.Subscribe(LoadState);
            
            // subscribe outer event to model change
            _model.Castle.CastleHealthReactiveProperty.Subscribe((newValue) =>
                _context.CastleHealthChangedReactiveCommand.Execute(new HealthInfo(newValue, _model.Castle.CastleMaxHealthReactiveProperty.Value)));
            _currencyController.GoldCoinsCountReactiveProperty.Subscribe((newValue) =>
                _context.GoldenCoinsCountChangedReactiveCommand.Execute(newValue));
            _currencyController.GoldCoinsIncomeReactiveProperty.Subscribe((newValue) =>
                _context.GoldenCoinsIncomeChangedReactiveCommand.Execute(newValue));
            _currencyController.CrystalsCountReactiveProperty.Subscribe((newValue) =>
                _context.CrystalsCountChangedReactiveCommand.Execute(newValue));
        }
        
        public void OuterLogicUpdate(float frameLength)
        { 
            if (_context.NeedsInput)
                _clicksDistributor.OuterLogicUpdate(frameLength);
            
            _constructionProcessController.OuterLogicUpdate(frameLength);
            _mobsManager.OuterLogicUpdate(frameLength);

            foreach (KeyValuePair<int,TowerController> towerPair in _model.Towers)
                towerPair.Value.OuterLogicUpdate(frameLength);
            
            _shootingController.OuterLogicUpdate(frameLength);
        }

        public void OuterViewUpdate(float frameLength)
        {
            _mobsManager.OuterViewUpdate(frameLength);
            _shootingController.OuterViewUpdate(frameLength);
        }

        private void CreateCells()
        {
            foreach (var hexModel in _context.MatchShortParameters.Hexes)
            {
                HexObject hexPrefab = _context.ConfigsRetriever.GetCellByType(hexModel.HexObjectTypeName);
                _factory.CreateCell(hexPrefab, hexModel.Position, hexModel.H);
            }
        }

        private void LoadState(PlayerState playerState)
        {
            _stateLoader.ClearState();
            _stateLoader.LoadState(playerState);
        }

        public PlayerState GetPlayerState()
        {
            return _stateLoader.SaveState();
        }
    }
}
