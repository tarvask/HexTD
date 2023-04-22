using System.Collections.Generic;

namespace Match.Wave
{
    public static class WaveBuilderInStrictOrder
    {
        public static List<WaveElementDelayAndPath> BuildWave(WaveParametersStrict waveParametersWithChances)
        {
            //PrepareWaveElements(waveParametersWithChances);
            List<WaveElementDelayAndPath> mobsWithDelays = new List<WaveElementDelayAndPath>(waveParametersWithChances.Elements.Length);
            // delays have absolute values in wave settings, but we need relative values
            float lastDelay = 0;

            foreach (WaveElementDelayAndPath waveElementWithDelay in waveParametersWithChances.Elements)
            {
                mobsWithDelays.Add(new WaveElementDelayAndPath(
                    waveElementWithDelay.MobId,
                    waveElementWithDelay.Delay - lastDelay,
                    waveElementWithDelay.PathId));
                lastDelay = waveElementWithDelay.Delay;
            }

            return mobsWithDelays;
        }
    }
}