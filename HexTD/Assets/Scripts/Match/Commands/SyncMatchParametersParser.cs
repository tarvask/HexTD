using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field.Hand;
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
            
            // level id
            byte levelId = (byte)parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigLevelIdParam];
            
            // waves
            // byte wavesCount = (byte)parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigWavesCount];
            // WaveWithDelayAndPath[] waves = new WaveWithDelayAndPath[wavesCount];
            //
            // for (byte currentWaveNumber = 0; currentWaveNumber < wavesCount; currentWaveNumber++)
            // {
            //     if (!parametersTable.TryGetValue(
            //         $"{PhotonEventsConstants.SyncMatch.MatchConfigWaveParam}{currentWaveNumber}", out object waveObject))
            //         break;
            //
            //     Hashtable waveHashtable = (Hashtable) waveObject;
            //     float waveDelay = (float)waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.WaveDelayParam];
            //     byte pathId = (byte)waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.PathIdParam];
            //     float duration = (float)waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.DurationParam];
            //     float pauseBeforeWave = (float)waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.PauseBeforeWaveParam];
            //     byte[] mobsIdsBytes = (byte[])waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.MobsIdsParam];
            //     float[] mobsDelaysBytes = (float[])waveHashtable[PhotonEventsConstants.SyncMatch.WaveStrictOrder.MobsDelaysParam];
            //     WaveElementDelay[] waveElementChances = new WaveElementDelay[mobsIdsBytes.Length];
            //
            //     for (int elementIndex = 0; elementIndex < mobsIdsBytes.Length; elementIndex++)
            //         waveElementChances[elementIndex] =
            //             new WaveElementDelay(
            //                 mobsIdsBytes[elementIndex],
            //                 mobsDelaysBytes[elementIndex]);
            //     
            //     WaveParametersStrict waveParameters = new WaveParametersStrict(duration, pauseBeforeWave, waveElementChances);
            //     waves[currentWaveNumber] = new WaveWithDelayAndPath(waveParameters, waveDelay, pathId);
            // }

            // cells
            // byte[] cellsBytes = (byte[])parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigFieldTypesParam];
            // HexModel[] hexModels = new HexModel[cellsBytes.Length];
            //
            // if (!parametersTable.TryGetValue(
            //         $"{PhotonEventsConstants.SyncMatch.MatchConfigHexFieldParam}", out object hexesModelsTableObject))
            //     throw new ArgumentException("Cannot parse hex models");
            //
            // Hashtable hexesModelsTable = (Hashtable)hexesModelsTableObject;
            //
            // for (byte currentHexIndex = 0; currentHexIndex < hexModels.Length; currentHexIndex++)
            // {
            //     if (!hexesModelsTable.TryGetValue(
            //             $"{currentHexIndex}", out object hexHashtableObject))
            //         break;
            //
            //     Hashtable hexHashtable = (Hashtable) hexHashtableObject;
            //     int q = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.Q];
            //     int r = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.R];
            //     int h = (int)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.H];
            //    
            //     byte dataLength = (byte)hexHashtable[PhotonEventsConstants.SyncMatch.HexStateParam.DataLength];
            //     Dictionary<string, string> hexData = new Dictionary<string, string>(dataLength);
            //
            //     for (int dataIndex = 0; dataIndex < dataLength; dataIndex++)
            //     {
            //         string key =
            //             (string)hexHashtable[$"{PhotonEventsConstants.SyncMatch.HexStateParam.DataKey}{dataIndex}"];
            //         string value =
            //             (string)hexHashtable[$"{PhotonEventsConstants.SyncMatch.HexStateParam.DataValue}{dataIndex}"];
            //         hexData.Add(key, value);
            //     }
            //
            //     hexModels[currentHexIndex] = new HexModel(new Hex2d(q, r), h, hexData);
            // }
            
            // paths
            // byte pathCount = (byte)parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigPathsCount];
            // PathData.SavePathData[] paths = new PathData.SavePathData[pathCount];
            //
            // for (byte pathIndex = 0; pathIndex < pathCount; pathIndex++)
            // {
            //     if (!parametersTable.TryGetValue(
            //             $"{PhotonEventsConstants.SyncMatch.MatchConfigPathFieldParam}{pathIndex}", out object pathHashtableObject))
            //         break;
            //
            //     Hashtable pathHashtable = (Hashtable) pathHashtableObject;
            //     byte name = (byte)pathHashtable[PhotonEventsConstants.SyncMatch.PathData.Name];
            //     byte pathSize = (byte)pathHashtable[PhotonEventsConstants.SyncMatch.PathData.PointLength];
            //
            //     List<Hex2d> points = new List<Hex2d>();
            //     for (int pointIndex = 0; pointIndex < pathSize; pointIndex++)
            //     {
            //         int q = (int)pathHashtable[$"{PhotonEventsConstants.SyncMatch.PathData.PointQ}{pointIndex}"];
            //         int r = (int)pathHashtable[$"{PhotonEventsConstants.SyncMatch.PathData.PointR}{pointIndex}"];
            //         points.Add(new Hex2d(q, r));
            //     }
            //
            //     paths[pathIndex] = new PathData.SavePathData(name, points);
            // }
            
            // coins
            // int startCoins = (int)parametersTable[PhotonEventsConstants.SyncMatch.MatchStartCoinsParam];

            // energy start
            // int energyStart = (int)parametersTable[PhotonEventsConstants.SyncMatch.MatchStartEnergyParam];
            
            // energy max
            // int energyMax = (int)parametersTable[PhotonEventsConstants.SyncMatch.MatchMaxEnergyParam];
            
            // energy restore delay
            // float energyRestoreDelay = (float)parametersTable[PhotonEventsConstants.SyncMatch.MatchRestoreEnergyDelay];
            
            // energy restore value
            // int energyRestoreValue = (int)parametersTable[PhotonEventsConstants.SyncMatch.MatchRestoreEnergyValue];

            // hand
            // towers
            byte[] towersBytes = (byte[]) parametersTable[PhotonEventsConstants.SyncMatch.MatchConfigHandTowersParam];
            TowerType[] towers = new TowerType[towersBytes.Length];

            for (int towerIndex = 0; towerIndex < towersBytes.Length; towerIndex++)
            {
                towers[towerIndex] = (TowerType)towersBytes[towerIndex];
            }

            PlayerHandParams clientPlayerHand = new PlayerHandParams(towers);
            MatchInitDataParameters clientMatchParameters = new MatchInitDataParameters(levelId, clientPlayerHand);

            // random seed
            int randomSeed = (int) parametersTable[PhotonEventsConstants.SyncMatch.RandomSeed];
                    
            return new Parameters(rolesAndUsers, clientMatchParameters, randomSeed);
        }
    }
}