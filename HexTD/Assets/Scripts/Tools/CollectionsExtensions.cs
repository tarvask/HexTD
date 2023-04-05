using System.Collections.Generic;

namespace Tools
{
    public class CollectionsExtensions
    {
        public static void ShuffleList<T>(ref List<T> candidates)
        {
            int maxIndex = candidates.Count;
            
            for (int i = 0; i < candidates.Count; i++)
            {
                T temp = candidates[i];
                int randomIndex = Randomizer.GetRandomInRange(0, maxIndex);
                candidates[i] = candidates[randomIndex];
                candidates[randomIndex] = temp;
            }
        }
    }
}