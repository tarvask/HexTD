using System.Collections.Generic;
using BuffLogic;
using HexSystem;
using MapEditor;
using Match.Commands;
using Match.Field.Castle;
using Match.Field.Currency;
using Match.Field.Hand;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Services;
using Match.Field.Shooting;
using Match.Field.State;
using Match.Field.Tower;
using Match.Field.VFX;
using PathSystem;
using Services;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace Match.Field
{
    public class FieldController : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public Transform FieldRoot { get; }
            public HexObjectFabric HexObjectFabric { get; }
            public PropsObjectFabric PropsObjectFabric { get; }
            public FieldConfig FieldConfig { get; }
            public MatchConfig LevelConfig { get; }
            public LevelMapModel LevelMapModel { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public BuffManager BuffManager { get; }
            public VfxManager VfxManager { get; }
            public bool ShouldCreateMap { get; }

            public ReactiveCommand<PlayerState> StateSyncedReactiveCommand { get; }
            public ReactiveCommand<MobSpawnParameters> SpawnMobReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnField { get; }
            public ReactiveCommand<HealthInfo> CastleHealthChangedReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }
            public ReactiveCommand<int> GoldenCoinsCountChangedReactiveCommand { get; }
            public ReactiveCommand<int> CrystalsCountChangedReactiveCommand { get; }

            public Context(
                Transform fieldRoot,
                HexObjectFabric hexObjectFabric,
                PropsObjectFabric propsObjectFabric,
                FieldConfig fieldConfig,
                MatchConfig levelConfig,
                LevelMapModel levelMapModel,
                ConfigsRetriever configsRetriever,
                BuffManager buffManager,
                VfxManager vfxManager,
                bool shouldCreateMap,
                
                ReactiveCommand<PlayerState> stateSyncedReactiveCommand,
                ReactiveCommand<MobSpawnParameters> spawnMobReactiveCommand,
                ReactiveProperty<bool> hasMobsOnField,
                ReactiveCommand<HealthInfo> castleHealthChangedReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand,
                ReactiveCommand<int> goldenCoinsCountChangedReactiveCommand,
                ReactiveCommand<int> crystalsCountChangedReactiveCommand)
            {
                FieldRoot = fieldRoot;
                HexObjectFabric = hexObjectFabric;
                PropsObjectFabric = propsObjectFabric;
                
                FieldConfig = fieldConfig;
                LevelConfig = levelConfig;
                LevelMapModel = levelMapModel;
                ConfigsRetriever = configsRetriever;
                BuffManager = buffManager;
                VfxManager = vfxManager;
                ShouldCreateMap = shouldCreateMap;

                StateSyncedReactiveCommand = stateSyncedReactiveCommand;
                SpawnMobReactiveCommand = spawnMobReactiveCommand;
                HasMobsOnField = hasMobsOnField;
                CastleHealthChangedReactiveCommand = castleHealthChangedReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
                GoldenCoinsCountChangedReactiveCommand = goldenCoinsCountChangedReactiveCommand;
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
        private readonly MobsByTowersBlocker _mobsByTowersBlocker;
        private readonly MeleeCombatManager _meleeCombatManager;
        private readonly MobsManager _mobsManager;
        
        private readonly FieldClicksHandler _clicksHandler;
        private readonly FieldClicksDistributor _clicksDistributor;
        private readonly FieldConstructionProcessController _constructionProcessController;
        private readonly ShootingProcessManager _shootingProcessManager;
        private readonly CurrencyController _currencyController;
        private readonly PlayerStateLoader _stateLoader;
        
        private readonly HexObjectsContainer _hexObjectsContainer ;
        private readonly PropsObjectsContainer _propsObjectsContainer ;
        private readonly FieldHighlightsController _fieldHighlightsController ;
        
        public const float MoveLerpCoeff = 0.7f;

        public HexagonalFieldModel HexagonalFieldModel => _hexagonalFieldModel;
        public PathContainer PathContainer => _pathContainer;

        public FieldConstructionProcessController FieldConstructionProcessController => _constructionProcessController;
        public FieldModel FieldModel => _model;

        private FieldFactory.Factory _fieldFactoryFactory;
        private TowersManager.Factory _towersManagerFactory;
        private MobsManager.Factory _mobsManagerFactory;

        public FieldController(
            Context context,
            FieldFactory.Factory fieldFactoryFactory,
            TowersManager.Factory towersManagerFactory,
            MobsManager.Factory mobsManagerFactory)
        {
            _context = context;
            _fieldFactoryFactory = fieldFactoryFactory;
            _towersManagerFactory = towersManagerFactory;
            _mobsManagerFactory = mobsManagerFactory;

            ReactiveCommand<MobController> attackTowerByMobReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<int> castleReachedByMobReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            ReactiveCommand<MobController> removeMobReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<MobController> mobSpawnedReactiveCommand = AddDisposable(new ReactiveCommand<MobController>());
            ReactiveCommand<int> crystalCollectedReactiveCommand = AddDisposable(new ReactiveCommand<int>());
            
            ReactiveCommand<IReadOnlyCollection<Hex2d>> enableHexesHighlightReactiveCommand = 
                AddDisposable(new ReactiveCommand<IReadOnlyCollection<Hex2d>>());
            ReactiveCommand removeAllHexesHighlightsReactiveCommand = 
                AddDisposable(new ReactiveCommand());

            _hexagonalFieldModel = new HexagonalFieldModel(_context.FieldConfig.HexSettingsConfig,
                _context.FieldRoot.position, _context.LevelMapModel.GetFieldHexes());
            _hexMapReachableService = new HexMapReachableService(_hexagonalFieldModel);
            
            _pathFindingService = new HexPathFindingService(_hexagonalFieldModel);
            _pathContainer = new PathContainer(_pathFindingService, _context.LevelMapModel.PathDatas);
            
            TowersManager towersManager = _towersManagerFactory.Create(
                _context.VfxManager, 
                _hexagonalFieldModel.HexGridSize);

            _hexObjectsContainer = new HexObjectsContainer();
            _propsObjectsContainer = new PropsObjectsContainer();
            
            _fieldHighlightsController = AddDisposable(new FieldHighlightsController(
                new FieldHighlightsController.Context(_hexObjectsContainer)));

            AddDisposable(enableHexesHighlightReactiveCommand.Subscribe(hexes =>
                _fieldHighlightsController.HighlightHexes(hexes)));
            AddDisposable(removeAllHexesHighlightsReactiveCommand.Subscribe(hexes =>
                _fieldHighlightsController.RemoveAllHighlights()));
            
            FieldFactory.Context factoryContext = new FieldFactory.Context(
                _context.FieldRoot,
                _context.HexObjectFabric,
                _context.PropsObjectFabric,
                _pathContainer,
                _hexagonalFieldModel,
                _context.LevelMapModel.GetFieldProps(),
                _context.LevelConfig.CastleHealth, 
                _context.FieldConfig.TowerRemovingDuration,
                _hexMapReachableService,
                _hexObjectsContainer,
                _propsObjectsContainer,
                castleReachedByMobReactiveCommand,
                _context.CastleDestroyedReactiveCommand,
                enableHexesHighlightReactiveCommand,
                removeAllHexesHighlightsReactiveCommand);
            _factory = AddDisposable(_fieldFactoryFactory.Create(factoryContext));
            
            MobsByTowersBlocker.Context mobsByTowersBlockerContext = new MobsByTowersBlocker.Context(
                _hexagonalFieldModel.HexSize, towersManager, removeMobReactiveCommand);
            _mobsByTowersBlocker = AddDisposable(new MobsByTowersBlocker(mobsByTowersBlockerContext));

            MobsManager.Context mobsManagerContext = new MobsManager.Context(
                _context.VfxManager,
                _mobsByTowersBlocker,
                _context.FieldConfig.RemoveMobsOnBossAppearing,
                attackTowerByMobReactiveCommand,
                castleReachedByMobReactiveCommand,
                removeMobReactiveCommand,
                _context.SpawnMobReactiveCommand);
            _mobsManager = AddDisposable(_mobsManagerFactory.Create(mobsManagerContext));
            
            FieldModel.Context fieldModelContext = new FieldModel.Context(
                _hexagonalFieldModel,
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

            MeleeCombatManager.Context meleeCombatManagerContext =
                new MeleeCombatManager.Context(_model, attackTowerByMobReactiveCommand);
            _meleeCombatManager = AddDisposable(new MeleeCombatManager(meleeCombatManagerContext));

            // construction
            FieldConstructionProcessController.Context constructionProcessControllerContext =
                new FieldConstructionProcessController.Context(_model, _factory);
            _constructionProcessController = AddDisposable(new FieldConstructionProcessController(constructionProcessControllerContext));
            
            // shooting
            ShootingProcessManager.Context shootingControllerContext = new ShootingProcessManager.Context(_model, 
                _hexMapReachableService, _factory, _context.BuffManager, _context.VfxManager);
            _shootingProcessManager = AddDisposable(new ShootingProcessManager(shootingControllerContext));
            
            // currency
            CurrencyController.Context currencyControllerContext = new CurrencyController.Context(
                100,
                5,
                 removeMobReactiveCommand, crystalCollectedReactiveCommand);
            _currencyController = AddDisposable(new CurrencyController(currencyControllerContext));

            // area buffs
            //FieldTowersAreaBuffsManager.Context towersAreaBuffsManagerContext = new FieldTowersAreaBuffsManager.Context(
            //    _model, towerBuiltReactiveCommand, towerPreUpgradedReactiveCommand, towerUpgradedReactiveCommand, towerRemovedReactiveCommand);
            //_towersAreaBuffsManager = AddDisposable(new FieldTowersAreaBuffsManager(towersAreaBuffsManagerContext));

            // clicks distribution

            PlayerStateLoader.Context stateLoaderContext = new PlayerStateLoader.Context(_model, _factory,
                _context.ConfigsRetriever,
                _currencyController);
            _stateLoader = AddDisposable(new PlayerStateLoader(stateLoaderContext));

            _context.StateSyncedReactiveCommand.Subscribe(LoadState);

            // subscribe outer event to model change
            _model.Castle.CastleHealthReactiveProperty.Subscribe((newValue) =>
                _context.CastleHealthChangedReactiveCommand.Execute(new HealthInfo(newValue, _model.Castle.CastleMaxHealthReactiveProperty.Value)));
            _currencyController.СoinsCountReactiveProperty.Subscribe((newValue) =>
                _context.GoldenCoinsCountChangedReactiveCommand.Execute(newValue));
            _currencyController.CrystalsCountReactiveProperty.Subscribe((newValue) =>
                _context.CrystalsCountChangedReactiveCommand.Execute(newValue));
            
            if (_context.ShouldCreateMap)
                _factory.CreateMap();
        }
        
        public void OuterLogicUpdate(float frameLength)
        { 
            _constructionProcessController.OuterLogicUpdate(frameLength);
            _mobsManager.OuterLogicUpdate(frameLength);
            _model.TowersManager.OuterLogicUpdate(frameLength);
            
            _shootingProcessManager.OuterLogicUpdate(frameLength);
        }

        public void OuterViewUpdate(float frameLength)
        {
            _mobsManager.OuterViewUpdate(frameLength);
            _shootingProcessManager.OuterViewUpdate(frameLength);
        }

        private void LoadState(PlayerState playerState)
        {
            _mobsByTowersBlocker.Clear();
            _stateLoader.ClearState();
            _stateLoader.LoadState(playerState);
        }

        public void InitPlayerHand(PlayerHandController playerHandController)
        {
            _model.TowersManager.TowerBuiltReactiveCommand.Subscribe(
                addedTower => playerHandController.RemoveTower(addedTower.TowerType));
            _model.TowersManager.TowerRemovedReactiveCommand.Subscribe(
                removedTower => playerHandController.AddTower(removedTower.TowerType));
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

        public class Factory : PlaceholderFactory<Context, FieldController>
        {
        }
    }
}
