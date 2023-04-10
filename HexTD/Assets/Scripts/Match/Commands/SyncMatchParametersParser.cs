using System.Collections.Generic;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field.Tower;
using Match.Wave;
using PathSystem;

namespace Match.Commands
{
    public static class SyncMatchParametersParser
    {
        public struct Parameters
        {
            public Dictionary<ProcessRoles, int> RolesAndUsers { get; }
            public MatchInitDataParameters ClientMatchParameters { get; }
            public int RandomSeed { get; }
            
            public Parameters(Dictionary<ProcessRoles, int> rolesAndUsers, 
                MatchInitDataParameters clientMatchParameters,
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
            byte[] cellsBytes = (byte[])parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigFieldTypesParam];
            HexModel[] hexModels = new HexModel[cellsBytes.Length];

            for (byte currentHexIndex = 0; currentHexIndex < hexModels.Length; currentHexIndex++)
            {
                if (!parametersTable.TryGetValue(
                        $"{PhotonEventsConstants.SyncMatch.MatchConfigHexFieldParam}{currentHexIndex}", out object hexHashtableObject))
                    break;

                Hashtable hexHashtable = (Hashtable) hexHashtableObject;
                int q = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.Q];
                int r = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.R];
                int h = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.H];
               
                byte dataLength = (byte)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.DataLength];
                Dictionary<string, string> hexData = new Dictionary<string, string>(dataLength);

                for (int dataIndex = 0; dataIndex < dataLength; dataIndex++)
                {
                    string key =
                        (string)hexHashtable[$"{PhotonEventsConstants.SyncMatch.HexStateParam.DataKey}{dataIndex}"];
                    string value =
                        (string)hexHashtable[$"{PhotonEventsConstants.SyncMatch.HexStateParam.DataValue}{dataIndex}"];
                    hexData.Add(key, value);
                }

                hexModels[currentHexIndex] = new HexModel(new Hex2d(q, r), h, hexData);
            }
            
            // paths
            byte pathCount = (byte)parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigPathsCount];
            PathData.SavePathData[] paths = new PathData.SavePathData[pathCount];

            for (byte pathIndex = 0; pathIndex < pathCount; pathIndex++)
            {
                if (!parametersTable.TryGetValue(
                        $"{PhotonEventsConstants.SyncMatch.MatchConfigPathsCount}{pathIndex}", out object pathHashtableObject))
                    break;

                Hashtable pathHashtable = (Hashtable) pathHashtableObject;
                string name = (string)pathHashtable[PhotonEventsConstants.SyncMatch.PathData.Name];
                byte pathSize = (byte)pathHashtable[PhotonEventsConstants.SyncMatch.PathData.PointLength];

                List<Hex2d> points = new List<Hex2d>();
                for (int pointIndex = 0; pointIndex < pathSize; pointIndex++)
                {
                    int q = (int)pathHashtable[$"{PhotonEventsConstants.SyncMatch.PathData.PointQ}{pointIndex}"];
                    int r = (int)pathHashtable[$"{PhotonEventsConstants.SyncMatch.PathData.PointR}{pointIndex}"];
                    points.Add(new Hex2d(q, r));
                }

                paths[pathIndex] = new PathData.SavePathData(name, points);
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
            MatchInitDataParameters clientMatchParameters = new MatchInitDataParameters(hexModels, paths, waves, silverCoins, clientPlayerHand);

            // random seed
            int randomSeed = (int) parametersTable[PhotonEventsConstants.SyncMatch.RandomSeed];
                    
            return new Parameters(rolesAndUsers, clientMatchParameters, randomSeed);
        }
    }
}