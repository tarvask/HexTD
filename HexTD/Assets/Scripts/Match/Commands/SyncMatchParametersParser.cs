using System.Collections.Generic;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field;
using Match.Field.Hexagonal;
using Match.Field.Tower;
using Match.Wave;

namespace Match.Commands
{
    public static class SyncMatchParametersParser
    {
        public struct Parameters
        {
            public Dictionary<ProcessRoles, int> RolesAndUsers { get; }
            public MatchParameters ClientMatchParameters { get; }
            public int RandomSeed { get; }
            
            public Parameters(Dictionary<ProcessRoles, int> rolesAndUsers, MatchParameters clientMatchParameters,
                int randomSeed)

            {
                RolesAndUsers = rolesAndUsers;
                ClientMatchParameters = clientMatchParameters;
                RandomSeed = randomSeed;
            }
        }

        public static Parameters ParseParameters(Hashtable parametersTable)
        {
            // roles and users
            Dictionary<byte, int> rolesAndUsersBytes = (Dictionary<byte, int>) parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigRolesAndUsers];
            Dictionary<ProcessRoles, int> rolesAndUsers = new Dictionary<ProcessRoles, int>(rolesAndUsersBytes.Count);

            foreach (KeyValuePair<byte, int> roleAndUserPair in rolesAndUsersBytes)
            {
                rolesAndUsers.Add((ProcessRoles)roleAndUserPair.Key, roleAndUserPair.Value);
            }
            
            // waves
            byte wavesCount = (byte)parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigWavesCount];
            WaveParams[] waves = new WaveParams[wavesCount];
            
            for (byte currentWaveNumber = 0; currentWaveNumber < wavesCount; currentWaveNumber++)
            {
                if (!parametersTable.TryGetValue(
                    $"{PhotonEventsConstants.SyncMatch.MatchConfigWaveParam}{currentWaveNumber}", out object waveObject))
                    break;

                Hashtable waveHashtable = (Hashtable) waveObject;
                byte size = (byte)waveHashtable[PhotonEventsConstants.SyncMatch.Wave.SizeParam];
                float duration = (float)waveHashtable[PhotonEventsConstants.SyncMatch.Wave.DurationParam];
                float spawnPauseMin = (float)waveHashtable[PhotonEventsConstants.SyncMatch.Wave.SpawnPauseMinParam];
                float spawnPauseMax = (float)waveHashtable[PhotonEventsConstants.SyncMatch.Wave.SpawnPauseMaxParam];
                float pauseBeforeWave = (float)waveHashtable[PhotonEventsConstants.SyncMatch.Wave.PauseBeforeWaveParam];
                byte[] mobsIdsBytes = (byte[])waveHashtable[PhotonEventsConstants.SyncMatch.Wave.MobsIdsParam];
                byte[] mobsCountsBytes = (byte[])waveHashtable[PhotonEventsConstants.SyncMatch.Wave.MobsCountsParam];
                WaveElementChance[] waveElementChances = new WaveElementChance [mobsIdsBytes.Length];

                for (int elementIndex = 0; elementIndex < mobsIdsBytes.Length; elementIndex++)
                    waveElementChances[elementIndex] =
                        new WaveElementChance(mobsIdsBytes[elementIndex], mobsCountsBytes[elementIndex]);
                
                waves[currentWaveNumber] = new WaveParams(size, duration, spawnPauseMin, spawnPauseMax, pauseBeforeWave, waveElementChances);
            }

            // cells
            byte[] cellsBytes = (byte[])parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigFieldParam];
            FieldHex[] cells = new FieldHex[cellsBytes.Length];

            for (int cellIndex = 0; cellIndex < cellsBytes.Length; cellIndex++)
            {
                HexModel hexModel = new HexModel(new Hex2d(), 0);
                FieldHex hex = new FieldHex(hexModel, (FieldHexType)cellsBytes[cellIndex]);
                cells[cellIndex] = hex;
            }
            
            // silver coins
            int silverCoins = (int)parametersTable[PhotonEventsConstants.SyncMatch.MatchStartSilverCoinsParam];

            // hand
            // towers
            byte[] towersBytes = (byte[]) parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigHandTowersParam];
            TowerType[] towers = new TowerType[towersBytes.Length];

            for (int towerIndex = 0; towerIndex < towersBytes.Length; towerIndex++)
            {
                towers[towerIndex] = (TowerType)towersBytes[towerIndex];
            }

            PlayerHandParams clientPlayerHand = new PlayerHandParams(towers);
            MatchParameters clientMatchParameters = new MatchParameters(waves, cells, silverCoins, clientPlayerHand);

            // random seed
            int randomSeed = (int) parametersTable[PhotonEventsConstants.SyncMatch.RandomSeed];
                    
            return new Parameters(rolesAndUsers, clientMatchParameters, randomSeed);
        }
    }
}