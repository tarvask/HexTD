public static class PhotonEventsConstants
{
    public const string MatchStarted = "matchStarted";
        
    public static class SyncMatch
    {
        public const byte SyncMatchConfigOnStartEventId = 0;

        public const string MatchConfigWavesCount = "wavesCount";
        public const string MatchConfigWaveParam = "wave";
        
        public const string MatchConfigFieldTypesParam = "matchConfigFieldTypes";
        public const string MatchConfigHexFieldParam = "matchConfighHexField";

        public const string MatchConfigPathsCount = "matchConfighPathCount";
        public const string MatchConfigPathFieldParam = "matchConfighPathField";
        
        public const string MatchStartCoinsParam = "coins";
        public const string MatchConfigRolesAndUsers = "rolesAndUsers";
        public const string MatchConfigHandTowersParam = "matchConfigHandTowers";
        public const string RandomSeed = "randomSeed";

        public static class Wave
        {
            public const string SizeParam = "size";
            public const string DurationParam = "duration";
            public const string SpawnPauseMinParam = "spawnPauseMin";
            public const string SpawnPauseMaxParam = "spawnPauseMax";
            public const string ArtifactsParam = "artifacts";
            public const string PauseBeforeWaveParam = "planningPause";
            public const string MobsIdsParam = "mobsIds";
            public const string MobsCountsParam = "mobsCounts";
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
            }
        }

        public static class PlayerState
        {
            public const string PlayerId = "playerId";
            public const string Coins = "coins";
            public const string Crystals = "crystals";
            public const string RerollCount = "reroll";
                
            // artifacts
            public const string ArtifactsParam = "artifacts";
                
            public static class Artifacts
            {
                public const string ArtifactIdParam = "id";
                public const string TypeIdParam = "typeId";
                public const string TowerTypeParam = "towerType";
                public const string IsUsedParam = "used";
                public const string TowerIdParam = "towerId";
            }
                
            // castle
            public const string CastleParam = "castle";
                
            public static class Castle
            {
                public const string CurrentHealthParam = "curHealth";
                public const string MaxHealthParam = "maxHealth";
            }
                
            // blockers
            public const string BlockersParam = "blockers";

            public static class Blockers
            {
                public const string BlockerIdParam = "id";
                public const string TargetIdParam = "targetId";
                public const string PositionXParam = "xposition";
                public const string PositionYParam = "yposition";
                public const string BlockerTypeParam = "type";
                public const string CurrentHealthParam = "curHealth";
            }
                
            // towers
            public const string TowersParam = "towers";

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

            public static class Mobs
            {
                public const string MobIdParam = "id";
                public const string TargetIdParam = "targetId";
                public const string TypeIdParam = "typeId";
                public const string PositionXParam = "xposition";
                public const string PositionYParam = "yposition";
                public const string NextWaypointParam = "waypoint";
                public const string CurrentHealthParam = "curHealth";
            }
                
            // projectiles
            public const string ProjectilesParam = "projectiles";

            public static class Projectiles
            {
                public const string ProjectileIdParam = "id";
                public const string TowerIdParam = "towerId";
                public const string TargetIdParam = "targetId";
                public const string AttackIndex = "attackIndex";
                public const string PositionXParam = "xposition";
                public const string PositionYParam = "yposition";
                // can be computed by tower
                public const string SpeedParam = "speed";
                public const string HasSplashDamageParam = "hasSplash";
            }

            // magic
            public const string MagicSpellsParam = "magicSpells";

            public static class MagicSpells
            {
                public const string MagicSpellTypeParam = "type";
                public const string MagicSpellCountParam = "count";
                public const string MagicSpellTimerParam = "timer";
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

    public static class TargetBlocker
    {
        public const byte RequestEventId = 16;
        public const byte ApplyEventId = 17;

        public const string RoleParam = "role";
        public const string PositionXParam = "xposition";
        public const string PositionYParam = "yposition";
        public const string TimeParam = "timestamp";
    }
        
    public static class UntargetBlocker
    {
        public const byte RequestEventId = 18;
        public const byte ApplyEventId = 19;

        public const string RoleParam = "role";
        public const string PositionXParam = "xposition";
        public const string PositionYParam = "yposition";
        public const string TimeParam = "timestamp";
    }

    public static class StartWaveSpawn
    {
        public const byte ApplyEventId = 20;

        public const string DurationParam = "duration";
        public const string ArtifactsParam = "artifacts";
        public const string PauseBeforeWaveParam = "planningPause";
        public const string Player1WaveMobsIds = "player1mobsIds";
        public const string Player1WaveMobsDelays = "player1mobsDelays";
        public const string Player2WaveMobsIds = "player2mobsIds";
        public const string Player2WaveMobsDelays = "player2mobsDelays";
        public const string RandomSeed = "randomSeed";
        public const string TimeParam = "timestamp";
    }

    public static class SendReinforcements
    {
        public const byte RequestEventId = 21;
            
        public const string RoleParam = "role";
        public const string MobsIds = "mobsIds";
        public const string MobsCounts = "mobsCounts";
        public const string TimeParam = "timestamp";
    }

    public static class CastMagicSpell
    {
        public const byte RequestEventId = 22;
        public const byte ApplyEventId = 23;

        public const string RoleParam = "role";
        public const string SpellTypeParam = "type";
        public const string IsFreeParam = "isFree";
        public const string TimeParam = "timestamp";
    }

    public static class CollectCrystal
    {
        public const byte RequestEventId = 24;
        public const byte ApplyEventId = 25;

        public const string RoleParam = "role";
        public const string PositionXParam = "xposition";
        public const string PositionYParam = "yposition";
        public const string TimeParam = "timestamp";
    }

    public static class UseReinforcementsItem
    {
        public const byte EventId = 27;

        public const string RoleParam = "role";
        public const string MobIdParam = "mobId";
        public const string ToUseParam = "use";
        public const string TimeParam = "timestamp";
    }

    public static class ChooseArtifactItem
    {
        public const byte RequestEventId = 28;
        public const byte ApplyEventId = 29;

        public const string RoleParam = "role";
        public const string ArtifactTypeIdParam = "artifactTypeId";
        public const string TowerTypeParam = "towerType";
        public const string TimeParam = "timestamp";
    }

    public static class PutArtifactOnTower
    {
        public const byte RequestEventId = 30;
        public const byte ApplyEventId = 31;

        public const string RoleParam = "role";
        public const string ArtifactIdParam = "artifactId";
        public const string TowerIdParam = "towerId";
        public const string TimeParam = "timestamp";
    }
        
    public static class TakeArtifactOffFromTower
    {
        public const byte RequestEventId = 32;
        public const byte ApplyEventId = 33;

        public const string RoleParam = "role";
        public const string ArtifactIdParam = "artifactId";
        public const string TowerIdParam = "towerId";
        public const string TimeParam = "timestamp";
    }

    public static class RerollArtifacts
    {
        public const byte RequestEventId = 34;
        public const byte ApplyEventId = 35;
            
        public const string RoleParam = "role";
        public const string TimeParam = "timestamp";
    }
}