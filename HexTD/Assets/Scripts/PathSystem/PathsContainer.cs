using System.Collections;
using System.Collections.Generic;

namespace PathSystem
{
    public class PathsContainer<T> : IEnumerable<T> where T : PathData
    {
        protected readonly Dictionary<string, T> Paths;

        public T this[string name] => Paths[name];

        protected PathsContainer()
        {
            Paths = new Dictionary<string, T>();
        }

        public PathsContainer(List<T> paths)
        {
            Paths = new Dictionary<string, T>();
            foreach (var pathData in paths)
            {
                Paths.Add(pathData.Name, pathData);
            }
        }

        public bool TryGetPathData(string name, out T pathData)
        {
            return Paths.TryGetValue(name, out pathData);
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