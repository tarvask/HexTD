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
        public WaveParametersStrict[] Waves { get; }
        //public int CoinsCount { get; }
        public int EnergyStartCount { get; }
        public PlayerHandParams PlayerHandParams { get; }
        
        public MatchInitDataParameters(HexModel[] hexes,
            PathData.SavePathData[] paths,
            WaveParametersStrict[] waves,
            //int coinsCount,
            int energyStartCount,
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
            Waves = new WaveParametersStrict[waves.Length];
            Array.Copy(waves, Waves, waves.Length);
            
            // currency and magic
            //CoinsCount = coinsCount;
            EnergyStartCount = energyStartCount;
            
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