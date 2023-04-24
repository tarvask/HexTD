using System;
using System.Collections.Generic;
using HexSystem;
using MapEditor;
using Match.Field.AttackEffect;
using Match.Field.Castle;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using PathSystem;
using Tools;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field
{
    public class FieldFactory : BaseDisposable
    {
        public struct Context
        {
            public Transform FieldRoot { get; }
            public HexFabric HexFabric { get; }
            public PathContainer PathContainer { get; }
            public HexagonalFieldModel HexagonalFieldModel { get; }
            public int CastleHealth { get; }
            public int TowerRemovingDuration { get; }
            public HexMapReachableService HexMapReachableService { get; }
            public HexObjectsContainer HexObjectsContainer { get; }
            
            public ReactiveCommand<int> ReachCastleByMobReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }
            public ReactiveCommand<IReadOnlyCollection<Hex2d>> EnableHexesHighlightReactiveCommand { get; }
            public ReactiveCommand RemoveAllHexesHighlightsReactiveCommand { get; }

            public Context(Transform fieldRoot,
                HexFabric hexFabric,
                PathContainer pathContainer,
                HexagonalFieldModel hexagonalFieldModel,
                int castleHealth, int towerRemovingDuration,
                HexMapReachableService hexMapReachableService,
                HexObjectsContainer hexObjectsContainer,
                ReactiveCommand<int> reachCastleByMobReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand,
                ReactiveCommand<IReadOnlyCollection<Hex2d>> enableHexesHighlightReactiveCommand ,
                ReactiveCommand removeAllHexesHighlightsReactiveCommand
                )
            {
                FieldRoot = fieldRoot;
                HexFabric = hexFabric;
                PathContainer = pathContainer;
                HexagonalFieldModel = hexagonalFieldModel;

                CastleHealth = castleHealth;
                TowerRemovingDuration = towerRemovingDuration;
                HexMapReachableService = hexMapReachableService;
                HexObjectsContainer = hexObjectsContainer;
                ReachCastleByMobReactiveCommand = reachCastleByMobReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
                EnableHexesHighlightReactiveCommand = enableHexesHighlightReactiveCommand;
                RemoveAllHexesHighlightsReactiveCommand = removeAllHexesHighlightsReactiveCommand;
            }
        }

        private readonly Context _context;

        private Transform _groundRoot;
        private Transform _hexsRoot;
        private Transform _buildingsRoot;
        private Transform _crystalsRoot;
        private Transform _mobsRoot;
        private Transform _projectilesRoot;
        
        private int _lastBuildingId;
        private int _lastCrystalId;
        private int _lastMobId;
        private int _lastProjectileId;
        private int _lastArtifactId;
        // TODO: use for blockers on map 
        private int _lastTargetId;

        public FieldFactory(Context context)
        {
            _context = context;
            PrepareFieldContainers();
        }

        private void PrepareFieldContainers()
        {
            // create cells root
            _groundRoot = AddComponent(new GameObject("Ground")).transform;
            _groundRoot.SetParent(_context.FieldRoot);
            _groundRoot.SetAsFirstSibling();
            _groundRoot.localPosition = Vector3.zero;
            _groundRoot.localScale = Vector3.one;
            
            // create cells root
            _hexsRoot = AddComponent(new GameObject("Cells")).transform;
            _hexsRoot.SetParent(_context.FieldRoot);
            _hexsRoot.SetAsLastSibling();
            _hexsRoot.localPosition = Vector3.zero;
            _hexsRoot.localScale = Vector3.one;
            
            // create buildings root
            _buildingsRoot = AddComponent(new GameObject("Buildings")).transform;
            _buildingsRoot.SetParent(_context.FieldRoot);
            _buildingsRoot.SetAsLastSibling();
            _buildingsRoot.localPosition = Vector3.zero;
            _buildingsRoot.localScale = Vector3.one;
            
            // create crystals root
            _crystalsRoot = AddComponent(new GameObject("Crystals")).transform;
            _crystalsRoot.SetParent(_context.FieldRoot);
            _crystalsRoot.SetAsLastSibling();
            _crystalsRoot.localPosition = Vector3.zero;
            _crystalsRoot.localScale = Vector3.one;
            
            // create mobs root
            _mobsRoot = AddComponent(new GameObject("Mobs")).transform;
            _mobsRoot.SetParent(_context.FieldRoot);
            _mobsRoot.SetAsLastSibling();
            _mobsRoot.localPosition = Vector3.zero;
            _mobsRoot.localScale = Vector3.one;
            
            // create projectiles root
            _projectilesRoot = AddComponent(new GameObject("Projectiles")).transform;
            _projectilesRoot.SetParent(_context.FieldRoot);
            _projectilesRoot.SetAsLastSibling();
            _projectilesRoot.localPosition = Vector3.zero;
            _projectilesRoot.localScale = Vector3.one;
        }

        public TowerController CreateTower(TowerConfigNew towerConfig,
            Hex2d position)
        {
            _lastBuildingId++;
            _lastTargetId++;
            
            return CreateTowerWithId(towerConfig, position, _lastBuildingId, _lastTargetId);
        }
        
        public TowerController CreateTowerWithId(TowerConfigNew towerConfig,
            Hex2d position, int towerId, int targetId)
        {
            if (_lastBuildingId < towerId)
                _lastBuildingId = towerId;

            if (_lastTargetId < targetId)
                _lastTargetId = targetId;
            
            TowerView towerView = CreateTowerView(towerConfig.RegularParameters.TowerName, 
                towerId, towerConfig.View, position);
            TowerController.Context towerControllerContext = new TowerController.Context(towerId, targetId,
                position, towerConfig, towerView, towerConfig.Icon, _context.TowerRemovingDuration,
                _context.HexMapReachableService,
                _context.EnableHexesHighlightReactiveCommand,
                _context.RemoveAllHexesHighlightsReactiveCommand);
            TowerController towerController = new TowerController(towerControllerContext);

            return towerController;
        }
        
        private TowerView CreateTowerView(string towerName, int towerId, TowerView towerPrefab,
            Hex2d position)
        {
            TowerView towerView = Object.Instantiate(towerPrefab, _buildingsRoot);
            towerView.transform.localPosition = _context.HexagonalFieldModel.GetHexPosition(position, false);
            towerView.name = $"{towerId}_{towerName}";

            return towerView;
        }

        public TowerView CreateTowerView(TowerConfigNew towerConfig)
        {
            TowerView towerView = Object.Instantiate(towerConfig.View, _buildingsRoot);
            towerView.name = $"{towerConfig.RegularParameters.TowerName}";

            return towerView;
        }

        public CastleController CreateCastle()
        {
            CastleController.Context castleContext = new CastleController.Context(_context.CastleHealth,
                _context.ReachCastleByMobReactiveCommand, _context.CastleDestroyedReactiveCommand);
            CastleController castle = new CastleController(castleContext);
            
            return castle;
        }

        public MobController CreateMob(MobSpawnParameters mobSpawnParameters,
            Vector3 spawnPosition)
        {
            _lastMobId++;
            _lastTargetId++;

            return CreateMobWithId(mobSpawnParameters, _lastMobId, _lastTargetId, spawnPosition);
        }

        public MobController CreateMobWithId(MobSpawnParameters mobSpawnParameters, int mobId, int targetId,
            Vector3 hexSpawnPosition)
        {
            if (_lastMobId < mobId)
                _lastMobId = mobId;

            if (_lastTargetId < targetId)
                _lastTargetId = targetId;

            if (!_context.PathContainer.TryGetPathData(mobSpawnParameters.PathId, out PathData pathData))
                throw new ArgumentException($"Unknown path name in mob's Parameters - in [{mobSpawnParameters.MobConfig.name}] prefab");

            MobView mobView = CreateMobView($"{mobSpawnParameters.MobConfig.Parameters.PowerType}",
                mobId, mobSpawnParameters.MobConfig.View, hexSpawnPosition);
            MobController.Context mobControllerContext = new MobController.Context(mobId, targetId,
                mobSpawnParameters.MobConfig.Parameters,
                pathData.GetPathEnumerator(), 
                _context.HexagonalFieldModel,
                mobView);
            MobController mobController = new MobController(mobControllerContext);

            return mobController;
        }
        
        private MobView CreateMobView(string mobName, int mobId, MobView mobPrefab,
            Vector3 spawnPosition)
        {
            MobView mobView = Object.Instantiate(mobPrefab, _mobsRoot);
            mobView.transform.localPosition = spawnPosition;
            mobView.name = $"{mobId}_{mobName}";

            return mobView;
        }
        
        public ProjectileController CreateProjectile(BaseAttackEffect attack, int attackIndex,
            Vector3 spawnPosition, bool hasSplashDamage,
            int towerId, int targetId)
        {
            _lastProjectileId++;

            return CreateProjectileWithId(attack, attackIndex,
                _lastProjectileId, spawnPosition, hasSplashDamage, 
                towerId, targetId);
        }

        public ProjectileController CreateProjectileWithId(BaseAttackEffect baseTowerAttack,
            int attackIndex, int projectileId, Vector3 spawnPosition, bool hasSplashDamage, 
            int towerId, int targetId)
        {
            if (_lastProjectileId < projectileId)
                _lastProjectileId = projectileId;

            SplashShootType splashShootType = baseTowerAttack is BaseSplashAttack
                ? ((BaseSplashAttack)baseTowerAttack).SplashShootType
                : SplashShootType.ToTarget;
            
            ProjectileView projectileInstance = CreateProjectileView(projectileId, baseTowerAttack.ProjectileView, spawnPosition);
            ProjectileController.Context projectileControllerContext = new ProjectileController.Context(projectileId,
                projectileInstance,
                baseTowerAttack, attackIndex, splashShootType,
                baseTowerAttack.ProjectileSpeed, hasSplashDamage, towerId, targetId);
            ProjectileController projectileController = new ProjectileController(projectileControllerContext);

            return projectileController;
        }

        private ProjectileView CreateProjectileView(int projectileId, ProjectileView projectilePrefab, Vector3 spawnPosition)
        {
            ProjectileView projectileInstance = Object.Instantiate(projectilePrefab, _projectilesRoot);
            projectileInstance.transform.localPosition = spawnPosition;
            projectileInstance.name = $"{projectileId}_bullet";

            return projectileInstance;
        }

        public void CreateCells()
        {
            foreach (var fieldHex in _context.HexagonalFieldModel)
            {
                CreateHexTile(fieldHex.Value.HexModel);
            }
        }

        public void CreateHexTile(HexModel hexModel)
        {
            Vector3 spawnPosition = _context.HexagonalFieldModel.GetHexPosition((Hex3d)hexModel);

            var hexObject = _context.HexFabric.CreateHexObject(hexModel, _hexsRoot, spawnPosition);
            _context.HexObjectsContainer.HexObjects.Add(hexModel.GetHashCode(), hexObject);
        }
    }
}