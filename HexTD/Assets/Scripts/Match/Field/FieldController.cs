using System.Collections.Generic;
using BuffLogic;
using HexSystem;
using Match.Commands;
using Match.Field.Castle;
using Match.Field.Currency;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Services;
using Match.Field.Shooting;
using Match.Field.State;
using Match.Field.Tower;
using PathSystem;
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
            public Transform FieldRoot { get; }
            public HexFabric HexFabric { get; }
            public MatchInitDataParameters MatchInitDataParameters { get; }
            public FieldConfig FieldConfig { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public BuffManager BuffManager { get; }
            public WindowsManager WindowsManager { get; }
            public bool NeedsInput { get; }
            
            public MatchCommands MatchCommands { get; }

            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }
            public ReactiveCommand<Hex2d> ClickReactiveCommand { get; }
            public ReactiveCommand<PlayerState> StateSyncedReactiveCommand { get; }
            public ReactiveCommand<MobConfig> SpawnMobReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnField { get; }
            public ReactiveCommand<int> WaveNumberChangedReactiveCommand { get; }
            public ReactiveCommand WaveEndedReactiveCommand { get; }
            public ReactiveCommand<HealthInfo> CastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }
            public ReactiveCommand<int> GoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> GoldenCoinsIncomeChangedReactiveCommand { get; }
            public ReactiveCommand<int> CrystalsCountChangedReactiveCommand { get; }

            public Context(
                Transform fieldRoot,
                HexFabric hexFabric,
                MatchInitDataParameters matchInitDataParameters, FieldConfig fieldConfig,
                ConfigsRetriever configsRetriever,
                BuffManager buffManager,
                WindowsManager windowsManager,

                bool needsInput,
                
                MatchCommands matchCommands,
                
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty,
                ReactiveCommand<Hex2d> clickReactiveCommand,
                ReactiveCommand<PlayerState> stateSyncedReactiveCommand,
                ReactiveCommand<MobConfig> spawnMobReactiveCommand,
                ReactiveProperty<bool> hasMobsOnField,
                ReactiveCommand<int> waveNumberChangedReactiveCommand,
                ReactiveCommand waveEndedReactiveCommand,
                ReactiveCommand<HealthInfo> castleHealthChangedReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand,
                ReactiveCommand<int> goldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> goldenCoinsIncomeChangedReactiveCommand,
                ReactiveCommand<int> crystalsCountChangedReactiveCommand)
            {
                FieldRoot = fieldRoot;
                HexFabric = hexFabric;
                
                MatchInitDataParameters = matchInitDataParameters;
                FieldConfig = fieldConfig;
                ConfigsRetriever = configsRetriever;
                BuffManager = buffManager;
                WindowsManager = windowsManager;

                NeedsInput = needsInput;
                MatchCommands = matchCommands;
                
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
                ClickReactiveCommand = clickReactiveCommand;
                StateSyncedReactiveCommand = stateSyncedReactiveCommand;
                SpawnMobReactiveCommand = spawnMobReactiveCommand;
                HasMobsOnField = hasMobsOnField;
                WaveNumberChangedReactiveCommand = waveNumberChangedReactiveCommand;
                WaveEndedReactiveCommand = waveEndedReactiveCommand;
                CastleHealthChangedReactiveCommand = castleHealthChangedReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
                GoldenCoinsCountChangedReactiveCommand = goldenCoinsCountChangedReactiveCommand;
                GoldenCoinsIncomeChangedReactiveCommand = goldenCoinsIncomeChangedReactiveCommand;
                CrystalsCountChangedReactiveCommand = crystalsCountChangedReactiveCommand;
            }
        }

        private readonly Context _context;

        private readonly FieldFactory _factory;
        private readonly FieldModel _model;
        private readonly HexagonalFieldModel _hexagonalFieldModel;
        private readonly HexMapReachableService _hexMapReachableService;
        private readonly HexPathFindingService _pathFindingService;
        private readonly PathContainer _pathContainer;
        
        private readonly FieldMobSpawner _fieldMobSpawner;
        private readonly MobsManager _mobsManager;
        private readonly FieldClicksHandler _clicksHandler;
        private readonly FieldClicksDistributor _clicksDistributor;
        private readonly FieldConstructionProcessController _constructionProcessController;
        private readonly ShootingController _shootingController;
        private readonly CurrencyController _currencyController;
        private readonly PlayerStateLoader _stateLoader;
        
        public const float MoveLerpCoeff = 0.7f;

        public FieldController(Context context)
        {
            _context = context;
            
            ReactiveCommand<int> castleAttackedByMobReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<MobController> removeMobReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<MobController> mobSpawnedReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<int> crystalCollectedReactiveCommand = AddDisposable(new ReactiveCommand<int>());

            _hexagonalFieldModel = new HexagonalFieldModel(_context.FieldConfig.HexSettingsConfig,
                _context.FieldRoot.position, _context.MatchInitDataParameters.Hexes);
            _hexMapReachableService = new HexMapReachableService(_hexagonalFieldModel);
            
            _pathFindingService = new HexPathFindingService(_hexagonalFieldModel);
            _pathContainer = new PathContainer(_pathFindingService, _context.MatchInitDataParameters.Paths);
            
            TowersManager towersManager = new TowersManager(_hexagonalFieldModel.HexGridSize);

            FieldFactory.Context factoryContext = new FieldFactory.Context(
                _context.FieldRoot,
                _context.HexFabric,
                _pathContainer,
                _hexagonalFieldModel,
                _context.FieldConfig.CastleHealth, 
                _context.FieldConfig.TowerRemovingDuration,
                castleAttackedByMobReactiveCommand,
                _context.CastleDestroyedReactiveCommand,
                removeMobReactiveCommand);
            _factory = AddDisposable(new FieldFactory(factoryContext));
            
            MobsManager.Context mobMoverContext = new MobsManager.Context(castleAttackedByMobReactiveCommand);
            _mobsManager = AddDisposable(new MobsManager(mobMoverContext));
            
            FieldModel.Context fieldModelContext = new FieldModel.Context(
                _hexagonalFieldModel.CurrentFieldHexTypes,
                towersManager,
                _mobsManager,
                _factory,
                removeMobReactiveCommand,
                mobSpawnedReactiveCommand,
                _context.HasMobsOnField);
            _model = AddDisposable(new FieldModel(fieldModelContext));

            FieldMobSpawner.Context mobSpawnerContext = new FieldMobSpawner.Context(_model, _factory, 
                _context.SpawnMobReactiveCommand);
            _fieldMobSpawner = AddDisposable(new FieldMobSpawner(mobSpawnerContext));

            // we only need input for player field
            if (_context.NeedsInput)
            {
                FieldClicksHandler.Context clicksHandlerContext = new FieldClicksHandler.Context(
                    _context.ClickReactiveCommand);
                _clicksHandler = AddDisposable(new FieldClicksHandler(clicksHandlerContext));
            }
            
            // construction
            FieldConstructionProcessController.Context constructionProcessControllerContext =
                new FieldConstructionProcessController.Context(_model, _factory);
            _constructionProcessController = AddDisposable(new FieldConstructionProcessController(constructionProcessControllerContext));
            
            // shooting
            ShootingController.Context shootingControllerContext = new ShootingController.Context(_model, 
                _hexMapReachableService, _factory, _context.BuffManager);
            _shootingController = AddDisposable(new ShootingController(shootingControllerContext));
            
            // currency
            CurrencyController.Context currencyControllerContext = new CurrencyController.Context(_context.MatchInitDataParameters.SilverCoinsCount,
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
                    _currencyController, _context.MatchCommands,
                    _context.WindowsManager.TowerSelectionWindowController,
                    _context.WindowsManager.TowerManipulationWindowController,
                    _context.WindowsManager.TowerInfoWindowController);
            _clicksDistributor = AddDisposable(new FieldClicksDistributor(clicksDistributorContext));

            PlayerStateLoader.Context stateLoaderContext = new PlayerStateLoader.Context(_model, _factory,
                _context.ConfigsRetriever,
                _currencyController);
            _stateLoader = AddDisposable(new PlayerStateLoader(stateLoaderContext));

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
            
            _factory.CreateCells();
        }
        
        public void OuterLogicUpdate(float frameLength)
        { 
            if (_context.NeedsInput)
                _clicksDistributor.OuterLogicUpdate(frameLength);
            
            _constructionProcessController.OuterLogicUpdate(frameLength);
            _mobsManager.OuterLogicUpdate(frameLength);
            _model.TowersManager.OuterLogicUpdate(frameLength);
            
            _shootingController.OuterLogicUpdate(frameLength);
        }

        public void OuterViewUpdate(float frameLength)
        {
            _mobsManager.OuterViewUpdate(frameLength);
            _shootingController.OuterViewUpdate(frameLength);
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

        public Bounds GetFieldBounds() => _hexagonalFieldModel.GetBounds();

        public void Reset()
        {
            _hexagonalFieldModel.Reset();
        }
    }
}
