using System;
using HexSystem;
using Match.Field;
using Match.Field.Hand;
using Match.Wave;
using PathSystem;

namespace Match
{
    public class MatchInitDataParameters
    {
        public FieldHex[] Hexes { get; }
        public PathData.SavePathData[] Paths { get; }
        public WaveParams[] Waves { get; }
        public int SilverCoinsCount { get; }
        public PlayerHandParams PlayerHandParams { get; }
        
        public MatchInitDataParameters(HexModel[] hexes,
            PathData.SavePathData[] paths,
            WaveParams[] waves,
            int silverCoinsCount,
            PlayerHandParams playerHandParams)
        {
            // fill cells from linear array
            Hexes = new FieldHex[hexes.Length];
            for (int hexIndex = 0; hexIndex < hexes.Length; hexIndex++)
            {
                Hexes[hexIndex] = new FieldHex(hexes[hexIndex], FieldHexType.Free);
            }
            
            // fill paths from linear array
            Paths = new PathData.SavePathData[paths.Length];
            Array.Copy(paths, Paths, paths.Length);

            // fill waves from linear array
            Waves = new WaveParams[waves.Length];
            Array.Copy(waves, Waves, waves.Length);
            
            // currency and magic
            SilverCoinsCount = silverCoinsCount;
            // hand
            PlayerHandParams = playerHandParams;
        }

        public byte[] GetHexesTypes()
        {
            byte[] hexesTypes = new byte[Hexes.Length];
            for (int hexIndex = 0; hexIndex < Hexes.Length; hexIndex++)
            {
                hexesTypes[hexIndex] = (byte)Hexes[hexIndex].HexType;
            }

            return hexesTypes;
        }
    }
}