using System.Collections.Generic;
using HexSystem;
using Match.Field.Currency;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.State;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Services;
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
            public ConfigsRetriever ConfigsRetriever { get; }
            public CurrencyController CurrencyController { get; }

            public Context(FieldModel fieldModel, FieldFactory fieldFactory,
                ConfigsRetriever towerConfigRetriever,
                CurrencyController currencyController)
            {
                FieldModel = fieldModel;
                FieldFactory = fieldFactory;
                ConfigsRetriever = towerConfigRetriever;
                CurrencyController = currencyController;
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
                
                TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerState.Type);
                Hex2d towerHexPosition = new Hex2d(towerState.PositionQ, towerState.PositionR);
                TowerController towerController = _context.FieldFactory.CreateTowerWithId(towerConfig,
                    towerHexPosition, towerState.Id);
                towerController.LoadState(towerState);

                _context.FieldModel.AddTower(towerController, towerHexPosition);
            }

            // mobs
            for (int mobIndex = 0; mobIndex < playerState.Mobs.Mobs.Length; mobIndex++)
            {
                ref readonly PlayerState.MobState mobState = ref playerState.Mobs.Mobs[mobIndex];
                
                MobConfig mobConfig = _context.ConfigsRetriever.GetMobById(mobState.TypeId);
                Vector2 mobPosition = new Vector2(mobState.PositionX, mobState.PositionY);
                MobController mobController = _context.FieldFactory.CreateMobWithId(mobConfig,
                    mobState.Id, mobState.TargetId,
                    mobPosition);
                mobController.LoadState(mobState);
                
                _context.FieldModel.AddMob(mobController);
            }
            
            // projectiles
            for (int projectileIndex = 0; projectileIndex < playerState.Projectiles.ActiveProjectilesCount; projectileIndex++)
            {
                ref readonly PlayerState.ProjectileState projectileState = ref playerState.Projectiles.Projectiles[projectileIndex];
                
                if (projectileState.Id == 0 || projectileState.TowerId == 0)
                    Debug.LogError($"Somehow id = 0: projectile is {projectileState.Id}, tower is {projectileState.TowerId}");

                TowerType towerType = _context.FieldModel.Towers[projectileState.TowerId].TowerType;
                TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerType);
                Vector3 projectilePosition = new Vector3(projectileState.PositionX, projectileState.PositionY);
                ProjectileController projectileController = _context.FieldFactory.CreateProjectileWithId(
                    towerConfig.TowerAttackConfig.TowerAttacks[0],
                    projectileState.Id, projectilePosition,
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
            PlayerState.MobsState mobsState = new PlayerState.MobsState(_context.FieldModel.MobsManager.Mobs);
            
            // projectiles
            PlayerState.ProjectilesState projectilesState = new PlayerState.ProjectilesState(_context.FieldModel.Projectiles);

            return new PlayerState(0, silverCoins, crystals,
                castleState, towersState, mobsState, projectilesState);
        }
        
        public void ClearState()
        {
            // towers
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
                towerPair.Value.Dispose();
            
            _context.FieldModel.Towers.Clear();
            _context.FieldModel.TowersByPositions.Clear();

            // mobs
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.MobsManager.Mobs)
                mobPair.Value.Dispose();
            
            _context.FieldModel.MobsManager.Clear();
            
            // projectiles
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.Projectiles)
                projectilePair.Value.Dispose();
            
            _context.FieldModel.Projectiles.Clear();
            
            // shootables
            _context.FieldModel.Shootables.Clear();
        }
    }
}