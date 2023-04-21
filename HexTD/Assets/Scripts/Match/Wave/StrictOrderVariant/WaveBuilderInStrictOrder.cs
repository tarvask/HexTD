using System.Collections.Generic;
using Tools;

namespace Match.Wave
{
    public class WaveBuilderInStrictOrder
    {
        static WaveBuilderInStrictOrder()
        {
            
        }
        
        public static List<WaveElementDelay> BuildWave(WaveParametersStrict waveParametersWithChances)
        {
            //PrepareWaveElements(waveParametersWithChances);
            List<WaveElementDelay> mobsWithDelays = new List<WaveElementDelay>(waveParametersWithChances.Elements.Length);
            // delays have absolute values in wave settings, but we need relative values
            float lastDelay = 0;

            foreach (WaveElementDelay waveElementWithDelay in waveParametersWithChances.Elements)
            {
                mobsWithDelays.Add(new WaveElementDelay(
                    waveElementWithDelay.MobId,
                    waveElementWithDelay.Delay - lastDelay));
                lastDelay = waveElementWithDelay.Delay;
            }

            return mobsWithDelays;
        }
    }
}