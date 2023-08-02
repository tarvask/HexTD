using System.Collections.Generic;
using ExitGames.Client.Photon;
using Match.Field.Castle;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Tower;

namespace Match.Field.State
{
    public readonly struct PlayerState
    {
        private readonly int _playerId;
        
        // currencies
        private readonly int _coins;
        private readonly int _crystals;

        // castle
        private readonly CastleState _castleState;

        // towers
        private readonly TowersState _towersState;
        
        // mobs
        private readonly MobsState _mobsState;
        
        // projectiles
        private readonly ProjectilesState _projectilesState;
        
        //buffs
        private readonly Hashtable _buffManagerHashtable;

        public int PlayerId => _playerId;
        public int Coins => _coins;
        public int Crystals => _crystals;
        public CastleState Castle => _castleState;
        public TowersState Towers => _towersState;
        public MobsState Mobs => _mobsState;
        public ProjectilesState Projectiles => _projectilesState;
        public Hashtable BuffManagerHashtable => _buffManagerHashtable;
        
        public PlayerState(int playerId, int coins, int crystals,
            CastleState castleState, TowersState towersState, MobsState mobsState,
            ProjectilesState projectilesState,
            Hashtable buffManagerHashtable)
        {
            _playerId = playerId;
            _coins = coins;
            _crystals = crystals;
            
            _castleState = castleState;
            _towersState = towersState;
            _mobsState = mobsState;
            _projectilesState = projectilesState;
            _buffManagerHashtable = buffManagerHashtable;
        }

        public PlayerState(int playerId, int coins, int crystals, FieldModel fieldModel)
        {
            _playerId = playerId;
            _coins = coins;
            _crystals = crystals;
            
            _castleState = new CastleState(fieldModel.Castle);
            _towersState = new TowersState(fieldModel.TowersManager.Towers);
            _mobsState = new MobsState(fieldModel.MobsManager.Mobs);
            _projectilesState = new ProjectilesState(fieldModel.ProjectilesContainer.Projectiles);
            _buffManagerHashtable = fieldModel.BuffManager.ToNetwork();
        }

        public static PlayerState FromHashtable(Hashtable playerStateHashtable)
        {
            int playerId = (int) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.PlayerId];
            int coins = (int)playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.Coins];
            int crystals = (int) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.Crystals];
            
            CastleState castleState = CastleState.CastleFromHashtable((Hashtable) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.CastleParam]);
            TowersState towersState = TowersState.TowersFromHashtable((Hashtable) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.TowersParam]);
            MobsState mobsState = MobsState.MobsFromHashtable((Hashtable) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.MobsParam]);
            ProjectilesState projectilesState = ProjectilesState.ProjectilesFromHashtable((Hashtable) playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.ProjectilesParam]);
            Hashtable buffHashtable = (Hashtable)playerStateHashtable[PhotonEventsConstants.SyncState.PlayerState.BuffsParam];
            
            return new PlayerState(playerId, coins, crystals,
                castleState, towersState, mobsState, projectilesState, buffHashtable);
        }

        public static Hashtable ToHashtable(in PlayerState playerState)
        {
            return new Hashtable
            {
                {PhotonEventsConstants.SyncState.PlayerState.PlayerId, playerState.PlayerId},
                {PhotonEventsConstants.SyncState.PlayerState.Coins, playerState.Coins},
                {PhotonEventsConstants.SyncState.PlayerState.Crystals, playerState.Crystals},
                {PhotonEventsConstants.SyncState.PlayerState.CastleParam, CastleState.CastleToHashtable(playerState.Castle)},
                {PhotonEventsConstants.SyncState.PlayerState.TowersParam, TowersState.TowersToHashtable(playerState.Towers)},
                {PhotonEventsConstants.SyncState.PlayerState.MobsParam, MobsState.MobsToHashtable(playerState.Mobs)},
                {PhotonEventsConstants.SyncState.PlayerState.ProjectilesParam, ProjectilesState.ProjectilesToHashtable(playerState.Projectiles)},
                {PhotonEventsConstants.SyncState.PlayerState.BuffsParam, playerState.BuffManagerHashtable}
            };
        }

        public readonly struct CastleState
        {
            private readonly int _currentHealth;
            private readonly int _maxHealth;

            public int CurrentHealth => _currentHealth;
            public int MaxHealth => _maxHealth;

            public CastleState(int currentHealth, int maxHealth)
            {
                _currentHealth = currentHealth;
                _maxHealth = maxHealth;
            }

            public CastleState(CastleController castleController)
            {
                this = castleController.GetCastleState();
            }

            public static CastleState CastleFromHashtable(Hashtable castleStateHashtable)
            {
                int currentHealth = (int) castleStateHashtable[PhotonEventsConstants.SyncState.PlayerState.Castle.CurrentHealthParam];
                int maxHealth = (int) castleStateHashtable[PhotonEventsConstants.SyncState.PlayerState.Castle.MaxHealthParam];
                
                return new CastleState(currentHealth, maxHealth);
            }

            public static Hashtable CastleToHashtable(in CastleState castleState)
            {
                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.PlayerState.Castle.CurrentHealthParam, castleState.CurrentHealth},
                    {PhotonEventsConstants.SyncState.PlayerState.Castle.MaxHealthParam, castleState.MaxHealth}
                };
            }
        }

        public readonly struct TowersState
        {
            private readonly TowerState[] _towers;

            public TowerState[] Towers => _towers;

            private TowersState(ref TowerState[] towers)
            {
                _towers = towers;
            }

            public TowersState(IReadOnlyDictionary<int, TowerController> towersControllers)
            {
                int activeTowers = 0;
                
                foreach (KeyValuePair<int, TowerController> towerPair in towersControllers)
                    if (towerPair.Value.IsAlive)
                        activeTowers++;
                
                _towers = new TowerState[activeTowers];
                int towerIndex = 0;

                foreach (KeyValuePair<int, TowerController> towerPair in towersControllers)
                    if (towerPair.Value.IsAlive)
                        _towers[towerIndex++] = towerPair.Value.GetTowerState();
            }
            
            public static TowersState TowersFromHashtable(Hashtable towersHashtable)
            {
                TowerState[] towers = new TowerState[towersHashtable.Count];
                int towerIndex = 0;

                foreach (var towerEntry in towersHashtable)
                {
                    Hashtable towerEntryHashtable = (Hashtable) towerEntry.Value;
                    towers[towerIndex] = TowerState.TowerFromHashtable(towerEntryHashtable);
                    towerIndex++;
                }
                
                return new TowersState(ref towers);
            }

            public static Hashtable TowersToHashtable(in TowersState towersState)
            {
                Hashtable towersHashtable = new Hashtable();

                foreach (TowerState tower in towersState.Towers)
                    towersHashtable.Add(tower.Id, TowerState.TowerToHashtable(tower));

                return towersHashtable;
            }
        }

        public readonly struct TowerState
        {
            private readonly int _id;
            private readonly int _targetId;
            private readonly short _qPosition;
            private readonly short _rPosition;
            private readonly TowerType _type;
            private readonly byte _level;
            private readonly int _constructionTime;

            public int Id => _id;
            public int TargetId => _targetId;
            public short PositionQ => _qPosition;
            public short PositionR => _rPosition;
            public TowerType Type => _type;
            public byte Level => _level;
            public int ConstructionTime => _constructionTime;

            public TowerState(int id, int targetId, short qPosition, short rPosition, TowerType type, byte level, int constructionTime)
            {
                _id = id;
                _targetId = targetId;
                _qPosition = qPosition;
                _rPosition = rPosition;
                _type = type;
                _level = level;
                _constructionTime = constructionTime;
            }

            public static TowerState TowerFromHashtable(Hashtable towerHashtable)
            {
                int id = (int)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.TowerIdParam];
                int targetId = (int)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.TowerTargetIdParam];
                short qPosition = (short)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.PositionQParam];
                short rPosition = (short)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.PositionRParam];
                TowerType type = (TowerType)(byte)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.TowerTypeParam];
                byte level = (byte)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.TowerLevelParam];
                int constructionTime = (int)towerHashtable[PhotonEventsConstants.SyncState.PlayerState.Towers.TowerConstructionTimeParam];

                return new TowerState(id, targetId, qPosition, rPosition, type, level, constructionTime);
            }

            public static Hashtable TowerToHashtable(in TowerState towersState)
            {
                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.TowerIdParam, towersState.Id},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.TowerTargetIdParam, towersState.TargetId},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.PositionQParam, towersState.PositionQ},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.PositionRParam, towersState.PositionR},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.TowerTypeParam, (byte)towersState.Type},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.TowerLevelParam, towersState.Level},
                    {PhotonEventsConstants.SyncState.PlayerState.Towers.TowerConstructionTimeParam, towersState.ConstructionTime},
                };
            }
        }

        public readonly struct MobsState
        {
            private readonly MobState[] _mobs;

            public MobState[] Mobs => _mobs;

            private MobsState(ref MobState[] mobs)
            {
                _mobs = mobs;
            }
            
            public MobsState(IReadOnlyDictionary<int, MobController> mobsControllers)
            {
                _mobs = new MobState[mobsControllers.Count];
                int mobIndex = 0;

                foreach (KeyValuePair<int, MobController> mobPair in mobsControllers)
                    _mobs[mobIndex++] = mobPair.Value.GetMobState();
            }
            
            public static MobsState MobsFromHashtable(Hashtable mobsHashtable)
            {
                MobState[] mobs = new MobState[mobsHashtable.Count];
                int mobIndex = 0;

                foreach (var mobEntry in mobsHashtable)
                {
                    Hashtable mobEntryHashtable = (Hashtable) mobEntry.Value;
                    mobs[mobIndex] = MobState.MobFromHashtable(mobEntryHashtable);
                    mobIndex++;
                }
                
                return new MobsState(ref mobs);
            }

            public static Hashtable MobsToHashtable(in MobsState mobsState)
            {
                Hashtable mobsHashtable = new Hashtable();

                foreach (MobState mob in mobsState.Mobs)
                    mobsHashtable.Add(mob.Id, MobState.MobToHashtable(mob));

                return mobsHashtable;
            }
        }

        public readonly struct MobState
        {
            private readonly int _id;
            private readonly int _targetId;
            private readonly byte _typeId;
            private readonly float _xPosition;
            private readonly float _zPosition;
            private readonly byte _pathId;
            private readonly byte _nextWaypoint;
            private readonly float _currentHealth;
            private readonly int _blockerId;
            private readonly float _attackingTimer;
            private readonly bool _isHittingForTheFirstTime;

            public int Id => _id;
            public int TargetId => _targetId;
            public byte TypeId => _typeId;
            public float PositionX => _xPosition;
            public float PositionZ => _zPosition;
            public byte PathId => _pathId;
            public byte NextWaypoint => _nextWaypoint;
            public float CurrentHealth => _currentHealth;
            public int BlockerId => _blockerId;
            public float AttackingTimer => _attackingTimer;
            public bool IsHittingForTheFirstTime => _isHittingForTheFirstTime;

            public MobState(int id, int targetId, byte typeId, float xPosition, float zPosition,
                byte pathId, byte nextWaypoint,
                float currentHealth,
                int blockerId,
                float attackingTimer,
                bool isHittingForTheFirstTime)
            {
                _id = id;
                _targetId = targetId;
                _typeId = typeId;
                _xPosition = xPosition;
                _zPosition = zPosition;
                _pathId = pathId;
                _nextWaypoint = nextWaypoint;
                _currentHealth = currentHealth;
                _blockerId = blockerId;
                _attackingTimer = attackingTimer;
                _isHittingForTheFirstTime = isHittingForTheFirstTime;
            }

            public static MobState MobFromHashtable(Hashtable mobHashtable)
            {
                int id = (int)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.MobIdParam];
                int targetId = (int)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.TargetIdParam];
                byte typeId = (byte)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.TypeIdParam];
                float xPosition = (float)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.PositionXParam];
                float yPosition = (float)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.PositionZParam];
                byte pathId = (byte)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.PathIdParam];
                byte nextWaypoint = (byte)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.NextWaypointParam];
                float currentHealth = (float)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.CurrentHealthParam];
                int blockerId = (int)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.BlockerIdParam];
                float attackingTimer = (float)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.AttackingTimerParam];
                bool isHittingForTheFirstTime = (bool)mobHashtable[PhotonEventsConstants.SyncState.PlayerState.Mobs.IsHittingForTheFirstTimeParam];
                
                return new MobState(id, targetId, typeId, xPosition, yPosition, pathId, nextWaypoint, currentHealth,
                    blockerId, attackingTimer, isHittingForTheFirstTime);
            }

            public static Hashtable MobToHashtable(in MobState mobState)
            {
                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.MobIdParam, mobState.Id},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.TargetIdParam, mobState.TargetId},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.TypeIdParam, mobState.TypeId},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.PositionXParam, mobState.PositionX},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.PositionZParam, mobState.PositionZ},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.PathIdParam, mobState.PathId},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.NextWaypointParam, mobState.NextWaypoint},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.CurrentHealthParam, mobState.CurrentHealth},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.BlockerIdParam, mobState.BlockerId},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.AttackingTimerParam, mobState.AttackingTimer},
                    {PhotonEventsConstants.SyncState.PlayerState.Mobs.IsHittingForTheFirstTimeParam, mobState.IsHittingForTheFirstTime},
                };
            }
        }

        public readonly struct ProjectilesState
        {
            private readonly ProjectileState[] _projectiles;
            private readonly int _activeProjectilesCount;

            public ProjectileState[] Projectiles => _projectiles;
            public int ActiveProjectilesCount => _activeProjectilesCount;

            private ProjectilesState(ref ProjectileState[] projectiles)
            {
                _projectiles = projectiles;
                _activeProjectilesCount = _projectiles.Length;
            }
            
            public ProjectilesState(IReadOnlyDictionary<int, ProjectileController> projectilesControllers)
            {
                _projectiles = new ProjectileState[projectilesControllers.Count];
                int projectileIndex = 0;

                foreach (KeyValuePair<int, ProjectileController> projectilePair in projectilesControllers)
                    if (!projectilePair.Value.HasReachedTarget)
                        _projectiles[projectileIndex++] = projectilePair.Value.GetProjectileState();

                _activeProjectilesCount = projectileIndex;
            }
            
            public static ProjectilesState ProjectilesFromHashtable(Hashtable projectilesHashtable)
            {
                ProjectileState[] projectiles = new ProjectileState[projectilesHashtable.Count];
                int projectileIndex = 0;

                foreach (var projectileEntry in projectilesHashtable)
                {
                    Hashtable projectileEntryHashtable = (Hashtable) projectileEntry.Value;
                    projectiles[projectileIndex] = ProjectileState.ProjectileFromHashtable(projectileEntryHashtable);
                    projectileIndex++;
                }
                
                return new ProjectilesState(ref projectiles);
            }

            public static Hashtable ProjectilesToHashtable(in ProjectilesState projectilesState)
            {
                Hashtable projectilesHashtable = new Hashtable();

                for (int projectileIndex = 0; projectileIndex < projectilesState.ActiveProjectilesCount; projectileIndex++)
                    projectilesHashtable.Add(projectilesState.Projectiles[projectileIndex].Id,
                        ProjectileState.ProjectileToHashtable(projectilesState.Projectiles[projectileIndex]));

                return projectilesHashtable;
            }
        }

        public readonly struct ProjectileState
        {
            private readonly int _id;
            private readonly int _towerId;
            private readonly int _targetId;
            private readonly int _attackIndex;
            private readonly float _xPosition;
            private readonly float _zPosition;
            // can be computed by tower
            private readonly float _speed;
            private readonly bool _hasSplash;

            public int Id => _id;
            public int TowerId => _towerId;
            public int TargetId => _targetId;
            public int AttackIndex => _attackIndex;
            public float PositionX => _xPosition;
            public float PositionZ => _zPosition;
            public float Speed => _speed;
            public bool HasSplash => _hasSplash;

            public ProjectileState(int id, int towerId, int targetId, int attackIndex, float xPosition, float zPosition,
                float speed, bool hasSplash)
            {
                _id = id;
                _towerId = towerId;
                _targetId = targetId;
                _attackIndex = attackIndex;
                _xPosition = xPosition;
                _zPosition = zPosition;
                _speed = speed;
                _hasSplash = hasSplash;
            }
            
            public static ProjectileState ProjectileFromHashtable(Hashtable projectileHashtable)
            {
                int id = (int)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.ProjectileIdParam];
                int towerId = (int)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.TowerIdParam];
                int targetId = (int)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.TargetIdParam];
                int attackIndex = (int)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.AttackIndex];
                float xPosition = (float)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.PositionXParam];
                float zPosition = (float)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.PositionZParam];
                float speed = (float)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.SpeedParam];
                bool hasSplash = (bool)projectileHashtable[PhotonEventsConstants.SyncState.PlayerState.Projectiles.HasSplashDamageParam];
                
                return new ProjectileState(id, towerId, targetId, attackIndex, xPosition, zPosition,
                    speed, hasSplash);
            }

            public static Hashtable ProjectileToHashtable(in ProjectileState projectileState)
            {
                return new Hashtable
                {
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.ProjectileIdParam, projectileState.Id},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.TowerIdParam, projectileState.TowerId},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.TargetIdParam, projectileState.TargetId},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.AttackIndex, projectileState.AttackIndex},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.PositionXParam, projectileState.PositionX},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.PositionZParam, projectileState.PositionZ},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.SpeedParam, projectileState.Speed},
                    {PhotonEventsConstants.SyncState.PlayerState.Projectiles.HasSplashDamageParam, projectileState.HasSplash}
                };
            }
        }
    }
}