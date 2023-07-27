public static class PhotonEventsConstants
{
    public const string MatchStarted = "matchStarted";
        
    public static class SyncMatch
    {
        public const byte SyncMatchConfigOnStartEventId = 0;

        public const string MatchConfigLevelIdParam = "levelId";

        public const string MatchConfigWavesCount = "wavesCount";
        public const string MatchConfigWaveParam = "wave";
        
        public const string MatchConfigFieldTypesParam = "matchConfigFieldTypes";
        public const string MatchConfigHexFieldParam = "matchConfighHexField";

        public const string MatchConfigPathsCount = "matchConfighPathCount";
        public const string MatchConfigPathFieldParam = "matchConfighPathField";
        
        public const string MatchStartCoinsParam = "coins";
        public const string MatchStartEnergyParam = "energyStart";
        public const string MatchMaxEnergyParam = "energyMax";
        public const string MatchRestoreEnergyDelay = "restoreDelay";
        public const string MatchRestoreEnergyValue = "restoreValue";
        public const string MatchConfigRolesAndUsers = "rolesAndUsers";
        public const string MatchConfigHandTowersParam = "matchConfigHandTowers";
        public const string RandomSeed = "randomSeed";

        public static class WaveWithRandom
        {
            public const string SizeParam = "size";
            public const string DurationParam = "duration";
            public const string SpawnPauseMinParam = "spawnPauseMin";
            public const string SpawnPauseMaxParam = "spawnPauseMax";
            public const string PauseBeforeWaveParam = "planningPause";
            public const string MobsIdsParam = "mobsIds";
            public const string MobsCountsParam = "mobsCounts";
        }
        
        public static class WaveStrictOrder
        {
            public const string DurationParam = "duration";
            public const string PauseBeforeWaveParam = "planningPause";
            public const string WaveDelayParam = "waveDelay";
            public const string PathIdParam = "pathid";
            public const string MobsIdsParam = "mobsIds";
            public const string MobsDelaysParam = "mobsCounts";
        }

        public static class HexStateParam
        {
            public const string Q = "Q";
            public const string R = "R";
            public const string H = "H";
            public const string DataLength = "DataLength";
            public const string DataKey = "DataKey";
            public const string DataValue = "DataValue";
        }

        public static class PropsStateParam
        {
            public const string Q = "Q";
            public const string R = "R";
            public const string H = "H";
            public const string DataLength = "DataLength";
            public const string DataKey = "DataKey";
            public const string DataValue = "DataValue";
        }

        public static class PathData
        {
            public const string Name = "Name";
            public const string PointLength = "PointLength";
            public const string PointQ = "PointQ";
            public const string PointR = "PointR";
        }
    }

    public static class SyncState
    {
        public const byte RequestEventId = 2;
        public const byte ApplyEventId = 3;

        public const string MatchStateParam = "match";

        public static class MatchState
        {
            public const string Player1StateParam = "player1State";
            public const string Player2StateParam = "player2State";
            public const string WaveStateParam = "waveState";
            public const string RandomSeedParam = "currentSeed";
            public const string RandomCounterParam = "currentRandom";
        }

        public static class WavesState
        {
            public const string CurrentWaveNumberParam = "waveNumber";
            public const string StateParam = "state";
            public const string TargetPauseDurationParam = "targetPause";
            public const string CurrentPauseDurationParam = "currentPause";
            public const string SpawnTimerParam = "spawnTimer";
            public const string Player1WavesParam = "player1Waves";
            public const string Player2WavesParam = "player2Waves";

            public static class WaveState
            {
                public const string WaveElementsParam = "elements";
                public const string TargetWaveDurationParam = "targetWaveDuration";
                public const string CurrentWaveDuration = "currentWaveDuration";
                public const string TargetSpawnPause = "targetSpawnPause";
                public const string LastSpawnTime = "lastSpawnTime";
            }

            public static class WaveElementState
            {
                public const string MobIdParam = "modId";
                public const string DelayParam = "delay";
                public const string PathParam = "pathId";
            }
        }

        public static class PlayerState
        {
            public const string PlayerId = "playerId";
            public const string Coins = "coins";
            public const string Crystals = "crystals";
                
            // castle
            public const string CastleParam = "castle";
                
            public static class Castle
            {
                public const string CurrentHealthParam = "curHealth";
                public const string MaxHealthParam = "maxHealth";
            }

            // towers
            public const string TowersParam = "towers";
            public const string TowersSizeParam = "towerSize";

            public static class Towers
            {
                public const string TowerIdParam = "id";
                public const string TowerTargetIdParam = "targetId";
                public const string PositionQParam = "qposition";
                public const string PositionRParam = "rposition";
                public const string TowerTypeParam = "type";
                public const string TowerLevelParam = "level";
                public const string TowerConstructionTimeParam = "buildTime";
            }

            // mobs
            public const string MobsParam = "mobs";
            public const string MobsSizeParam = "mobSize";

            public static class Mobs
            {
                public const string MobIdParam = "id";
                public const string TargetIdParam = "targetId";
                public const string TypeIdParam = "typeId";
                public const string PositionXParam = "xposition";
                public const string PositionZParam = "zposition";
                public const string PathIdParam = "path";
                public const string NextWaypointParam = "waypoint";
                public const string CurrentHealthParam = "curHealth";
                public const string BlockerIdParam = "blockerid";
            }
                
            // projectiles
            public const string ProjectilesParam = "projectiles";
            public const string ProjectilesSizeParam = "projectileSize";

            public static class Projectiles
            {
                public const string ProjectileIdParam = "id";
                public const string TowerIdParam = "towerId";
                public const string TargetIdParam = "targetId";
                public const string AttackIndex = "attackIndex";
                public const string PositionXParam = "xposition";
                public const string PositionZParam = "zposition";
                // can be computed by tower
                public const string SpeedParam = "speed";
                public const string HasSplashDamageParam = "hasSplash";
            }
            
            // buffs
            public static class Buffs
            {
                public const string TargetId = "targetId";
                public const string EntityBuffableValueType = "entityBuffableValueType";
                
                public const string BuffConditionName = "BuffConditionParam";
                
                public const string BuffSizeParam = "buffCount";
                public const string BuffValueParam = "buffValue";

                public const string TypedBuffManagerSize = "typedBuffManagerSize";
                public const string TypedBuffManager = "typedBuffManager";
                public const string TypedBuffManagerType = "SerializedType";
                public const string BuffManager = "BuffManager";
            }
        }
            
        public const string TimeParam = "timestamp";
    }

    public static class BuildTower
    {
        public const byte RequestEventId = 10;
        public const byte ApplyEventId = 11;

        public const string RoleParam = "role";
        public const string PositionQParam = "qposition";
        public const string PositionRParam = "rposition";
        public const string TowerTypeParam = "type";
        public const string TowerLevelParam = "level";
        public const string TimeParam = "timestamp";
    }

    public static class UpgradeTower
    {
        public const byte RequestEventId = 12;
        public const byte ApplyEventId = 13;
            
        public const string RoleParam = "role";
        public const string PositionQParam = "qposition";
        public const string PositionRParam = "rposition";
        public const string TowerTypeParam = "type";
        public const string TowerLevelParam = "level";
        public const string TimeParam = "timestamp";
    }

    public static class SellTower
    {
        public const byte RequestEventId = 14;
        public const byte ApplyEventId = 15;
            
        public const string RoleParam = "role";
        public const string PositionQParam = "qposition";
        public const string PositionRParam = "rposition";
        public const string TowerTypeParam = "type";
        public const string TowerLevelParam = "level";
        public const string TimeParam = "timestamp";
    }

    public static class StartWaveSpawn
    {
        public const byte ApplyEventId = 20;

        public const string DurationParam = "duration";
        public const string PauseBeforeWaveParam = "planningPause";
        public const string Player1WaveMobsIds = "player1mobsIds";
        public const string Player1WaveMobsDelays = "player1mobsDelays";
        public const string Player1WaveMobsPaths = "player1mobsPaths";
        public const string Player2WaveMobsIds = "player2mobsIds";
        public const string Player2WaveMobsDelays = "player2mobsDelays";
        public const string Player2WaveMobsPaths = "player2mobsPaths";
        public const string RandomSeed = "randomSeed";
        public const string TimeParam = "timestamp";
    }
    
    public static class BroadcastStateCheckSum
    {
        public const byte ApplyEventId = 36;

        public const string Player1CheckSumParam = "player1checksum";
        public const string Player2CheckSumParam = "player2checksum";

        public const string TimeParam = "timestamp";
    }
}