using System.Collections.Generic;

namespace Match.Wave
{
    public static class WaveBuilderInStrictOrder
    {
        public static List<WaveElementDelayAndPath> BuildWave(WaveWithDelayAndPath waveParametersWithChances)
        {
            //PrepareWaveElements(waveParametersWithChances);
            List<WaveElementDelayAndPath> mobsWithDelays = new List<WaveElementDelayAndPath>(waveParametersWithChances.WaveParameters.Elements.Length);
            // delays have absolute values in wave settings, but we need relative values
            float lastDelay = 0;

            foreach (WaveElementDelay waveElementWithDelay in waveParametersWithChances.WaveParameters.Elements)
            {
                mobsWithDelays.Add(new WaveElementDelayAndPath(
                    waveElementWithDelay.MobId,
                    waveElementWithDelay.Delay - lastDelay,
                    waveParametersWithChances.PathId));
                lastDelay = waveElementWithDelay.Delay;
            }

            return mobsWithDelays;
        }
    }
}