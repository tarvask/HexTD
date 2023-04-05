using System.Collections.Generic;
using Match.Field.Currency;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.State;
using Match.Field.Tower;
using Tools;
using UnityEngine;

namespace Match.Field.Services
{
    public class PlayerStateLoader : BaseDisposable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public FieldFactory FieldFactory { get; }
            public TowerConfigRetriever TowerConfigRetriever { get; }
            public MobConfigRetriever MobConfigRetriever { get; }
            public CurrencyController CurrencyController { get; }
            public Vector3[] WayPoints { get; }

            public Context(FieldModel fieldModel, FieldFactory fieldFactory,
                TowerConfigRetriever towerConfigRetriever,
                MobConfigRetriever mobConfigRetriever,
                CurrencyController currencyController,
                Vector3[] wayPoints)
            {
                FieldModel = fieldModel;
                FieldFactory = fieldFactory;
                TowerConfigRetriever = towerConfigRetriever;
                MobConfigRetriever = mobConfigRetriever;
                CurrencyController = currencyController;
                WayPoints = wayPoints;
            }
        }

        private readonly Context _context;

        public PlayerStateLoader(Context context)
        {
            _context = context;
        }

        public void LoadState(PlayerState playerState)
        {
            // silver
            if (_context.CurrencyController.GoldCoinsCountReactiveProperty.Value > playerState.SilverCoins)
                _context.CurrencyController.SpendSilver(_context.CurrencyController.GoldCoinsCountReactiveProperty.Value - playerState.SilverCoins);
            else
                _context.CurrencyController.AddSilver(playerState.SilverCoins - _context.CurrencyController.GoldCoinsCountReactiveProperty.Value);
            
            // crystals
            if (_context.CurrencyController.CrystalsCountReactiveProperty.Value > playerState.CurrentCrystals)
                _context.CurrencyController.SpendCrystals(_context.CurrencyController.CrystalsCountReactiveProperty.Value - playerState.CurrentCrystals);
            else
                _context.CurrencyController.AddCrystals(_context.CurrencyController.CrystalsCountReactiveProperty.Value - playerState.CurrentCrystals);

            // castle
            _context.FieldModel.Castle.LoadState(playerState.Castle);

            // towers
            for (int towerIndex = 0; towerIndex < playerState.Towers.Towers.Length; towerIndex++)
            {
                ref readonly PlayerState.TowerState towerState = ref playerState.Towers.Towers[towerIndex];
                
                TowerConfig towerConfig = _context.TowerConfigRetriever.GetTowerByType(towerState.Type);
                Vector2Int towerPosition = new Vector2Int(towerState.PositionX, towerState.PositionY);
                TowerController towerController = _context.FieldFactory.CreateTowerWithId(towerConfig,
                    towerState.PositionX, towerState.PositionY, towerState.Id,
                    _context.FieldModel.IsCellNearRoad(towerPosition));
                towerController.LoadState(towerState);

                _context.FieldModel.AddTower(towerController, towerPosition);
            }

            // mobs
            for (int mobIndex = 0; mobIndex < playerState.Mobs.Mobs.Length; mobIndex++)
            {
                ref readonly PlayerState.MobState mobState = ref playerState.Mobs.Mobs[mobIndex];
                
                MobConfig mobConfig = _context.MobConfigRetriever.GetMobById(mobState.TypeId);
                Vector3 mobPosition = new Vector3(mobState.PositionX, mobState.PositionY);
                MobController mobController = _context.FieldFactory.CreateMobWithId(mobConfig,
                    mobState.Id, mobState.TargetId,
                    mobPosition, _context.WayPoints);
                mobController.LoadState(mobState);
                
                _context.FieldModel.AddMob(mobController);
            }
            
            // projectiles
            for (int projectileIndex = 0; projectileIndex < playerState.Projectiles.ActiveProjectilesCount; projectileIndex++)
            {
                ref readonly PlayerState.ProjectileState projectileState = ref playerState.Projectiles.Projectiles[projectileIndex];
                
                if (projectileState.Id == 0 || projectileState.TowerId == 0)
                    Debug.LogError($"Somehow id = 0: projectile is {projectileState.Id}, tower is {projectileState.TowerId}");
                    
                Vector3 projectilePosition = new Vector3(projectileState.PositionX, projectileState.PositionY);
                ProjectileController projectileController = _context.FieldFactory.CreateProjectileWithId(
                    _context.FieldModel.Towers[projectileState.TowerId].ProjectilePrefab,
                    projectileState.Id, projectilePosition, projectileState.Speed,
                    projectileState.HasSplash, projectileState.SplashRadius, projectileState.HasProgressiveSplash,
                    projectileState.TowerId, projectileState.TargetId);
                projectileController.LoadState(projectileState);
                
                _context.FieldModel.AddProjectile(projectileController);
            }
        }

        public PlayerState SaveState()
        {
            // silver
            int silverCoins = _context.CurrencyController.GoldCoinsCountReactiveProperty.Value;
            
            // crystals
            int crystals = _context.CurrencyController.CrystalsCountReactiveProperty.Value;

            // castle
            PlayerState.CastleState castleState = new PlayerState.CastleState(_context.FieldModel.Castle);

            // towers
            PlayerState.TowersState towersState = new PlayerState.TowersState(_context.FieldModel.Towers);
            
            // mobs
            PlayerState.MobsState mobsState = new PlayerState.MobsState(_context.FieldModel.Mobs);
            
            // projectiles
            PlayerState.ProjectilesState projectilesState = new PlayerState.ProjectilesState(_context.FieldModel.Projectiles);

            return new PlayerState(0, silverCoins, crystals,
                castleState, towersState, mobsState, projectilesState);
        }
        
        public void ClearState()
        {
            // cells
            _context.FieldModel.RestoreCells();

            // towers
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
                towerPair.Value.Dispose();
            
            _context.FieldModel.Towers.Clear();
            _context.FieldModel.TowersByPositions.Clear();

            // mobs
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.Mobs)
                mobPair.Value.Dispose();
            
            _context.FieldModel.Mobs.Clear();
            
            // projectiles
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.Projectiles)
                projectilePair.Value.Dispose();
            
            _context.FieldModel.Projectiles.Clear();
            
            // shootables
            _context.FieldModel.Shootables.Clear();
        }
    }
}