using System.Collections;
using System.Collections.Generic;

namespace PathSystem
{
    public class PathsContainer<T> : IEnumerable<T> where T : PathData
    {
        protected readonly Dictionary<byte, T> Paths;

        public T this[byte name] => Paths[name];

        protected PathsContainer()
        {
            Paths = new Dictionary<byte, T>();
        }

        public PathsContainer(List<T> paths)
        {
            Paths = new Dictionary<byte, T>();
            foreach (var pathData in paths)
            {
                Paths.Add(pathData.PathId, pathData);
            }
        }
        
        public bool TryGetPathData(byte pathId, out T pathData)
        {
            return Paths.TryGetValue(pathId, out pathData);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Paths.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}