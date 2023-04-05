using Match.Field.Castle;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldFactory : BaseDisposable
    {
        public struct Context
        {
            public Transform FieldRoot { get; }
            public Vector2 FieldBottomLeftCornerPosition { get; }
            public int CastleHealth { get; }
            public int TowerRemovingDuration { get; }
            
            public ReactiveCommand<int> AttackCastleByMobReactiveCommand { get; }
            public ReactiveCommand CastleDestroyedReactiveCommand { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }

            public Context(Transform fieldRoot, Vector2 fieldBottomLeftCornerPosition,
                int castleHealth, int towerRemovingDuration,
                ReactiveCommand<int> attackCastleByMobReactiveCommand,
                ReactiveCommand castleDestroyedReactiveCommand,
                ReactiveCommand<MobController> removeMobReactiveCommand)
            {
                FieldRoot = fieldRoot;
                FieldBottomLeftCornerPosition = fieldBottomLeftCornerPosition;

                CastleHealth = castleHealth;
                TowerRemovingDuration = towerRemovingDuration;
                AttackCastleByMobReactiveCommand = attackCastleByMobReactiveCommand;
                CastleDestroyedReactiveCommand = castleDestroyedReactiveCommand;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
            }
        }

        private readonly Context _context;

        private Transform _groundRoot;
        private Transform _cellsRoot;
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
            _cellsRoot = AddComponent(new GameObject("Cells")).transform;
            _cellsRoot.SetParent(_context.FieldRoot);
            _cellsRoot.SetAsLastSibling();
            _cellsRoot.localPosition = Vector3.zero;
            _cellsRoot.localScale = Vector3.one;
            
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

        public TowerController CreateTower(TowerConfig towerConfig,
            int cellX, int cellY, bool isCellNearRoad)
        {
            _lastBuildingId++;
            
            return CreateTowerWithId(towerConfig, cellX, cellY, _lastBuildingId, isCellNearRoad);
        }
        
        public TowerController CreateTowerWithId(TowerConfig towerConfig,
            int cellX, int cellY, int towerId, bool isCellNearRoad)
        {
            if (_lastBuildingId < towerId)
                _lastBuildingId = towerId;
            
            TowerView towerView = CreateTowerView(towerConfig.Parameters.RegularParameters.Data.TowerName, towerId, towerConfig.View, new Vector3(cellX, cellY));
            TowerController.Context towerControllerContext = new TowerController.Context(towerId,
                towerConfig.Parameters, towerView, towerConfig.Icon, _context.TowerRemovingDuration, isCellNearRoad);
            TowerController towerController = new TowerController(towerControllerContext);

            return towerController;
        }
        
        private TowerView CreateTowerView(string towerName, int towerId, TowerView towerPrefab,
            Vector3 position)
        {
            TowerView towerView = Object.Instantiate(towerPrefab, _buildingsRoot);
            towerView.transform.localPosition = position;
            towerView.name = $"{towerId}_{towerName}";

            return towerView;
        }

        public CastleController CreateCastle()
        {
            CastleController.Context castleContext = new CastleController.Context(_context.CastleHealth,
                _context.AttackCastleByMobReactiveCommand, _context.CastleDestroyedReactiveCommand);
            CastleController castle = new CastleController(castleContext);
            
            return castle;
        }

        public MobController CreateMob(MobConfig mobConfig,
            Vector3 spawnPosition, Vector3[] waypoints)
        {
            _lastMobId++;
            _lastTargetId++;

            return CreateMobWithId(mobConfig, _lastMobId, _lastTargetId, spawnPosition, waypoints);
        }

        public MobController CreateMobWithId(MobConfig mobConfig, int mobId, int targetId,
            Vector3 spawnPosition, Vector3[] waypoints)
        {
            if (_lastMobId < mobId)
                _lastMobId = mobId;

            if (_lastTargetId < targetId)
                _lastTargetId = targetId;
            
            MobView mobView = CreateMobView($"{mobConfig.Parameters.PowerType}",
                mobId, mobConfig.View, spawnPosition);
            MobController.Context towerControllerContext = new MobController.Context(mobId, targetId,
                mobConfig.Parameters, mobView, waypoints, _context.RemoveMobReactiveCommand);
            MobController mobController = new MobController(towerControllerContext);

            return mobController;
        }
        
        private MobView CreateMobView(string mobName, int mobId, MobView mobPrefab,
            Vector3 position)
        {
            MobView mobView = Object.Instantiate(mobPrefab, _mobsRoot);
            mobView.transform.localPosition = position;
            mobView.name = $"{mobId}_{mobName}";

            return mobView;
        }

        public GameObject CreateGroundElement(GameObject groundPrefab, int cellX, int cellY)
        {
            GameObject groundInstance = Object.Instantiate(groundPrefab, _groundRoot);
            groundInstance.transform.localPosition = new Vector3(cellX, cellY);
            return groundInstance;
        }

        public GameObject CreateCell(GameObject cellPrefab, int cellX, int cellY)
        {
            GameObject cellInstance = Object.Instantiate(cellPrefab, _cellsRoot);
            cellInstance.transform.localPosition = new Vector3(cellX, cellY);
            return cellInstance;
        }
        
        public ProjectileController CreateProjectile(ProjectileView projectilePrefab, Vector3 spawnPosition,
            float projectileSpeed, bool hasSplashDamage, float splashDamageRadius, bool hasProgressiveSplash,
            int towerId, int targetId)
        {
            _lastProjectileId++;

            return CreateProjectileWithId(projectilePrefab, _lastProjectileId, spawnPosition,
                projectileSpeed, hasSplashDamage, splashDamageRadius, hasProgressiveSplash,
                towerId, targetId);
        }

        public ProjectileController CreateProjectileWithId(ProjectileView projectilePrefab, int projectileId, Vector3 spawnPosition,
            float projectileSpeed, bool hasSplashDamage, float splashDamageRadius, bool hasProgressiveSplash,
            int towerId, int targetId)
        {
            if (_lastProjectileId < projectileId)
                _lastProjectileId = projectileId;
            
            ProjectileView projectileInstance = CreateProjectileView(projectileId, projectilePrefab, spawnPosition);
            ProjectileController.Context projectileControllerContext = new ProjectileController.Context(projectileId,
                projectileInstance, projectileSpeed, hasSplashDamage, splashDamageRadius, hasProgressiveSplash,
                towerId, targetId);
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
    }
}