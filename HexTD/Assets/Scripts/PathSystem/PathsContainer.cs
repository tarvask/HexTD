using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathSystem
{
    public class PathsContainer<T> : IEnumerable<T> where T : PathData
    {
        protected readonly Dictionary<string, T> Paths;

        public T this[string name] => Paths[name];
        
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

        public List<PathData.SavePathData> GetPaths()
        {
            var savePathDatas = new List<PathData.SavePathData>();
            foreach (var path in Paths.Values)
            {
                savePathDatas.Add(path.GetSavePathData());
            }
            
            return savePathDatas;
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

    public class PathContainer : PathsContainer<PathData>
    {
        public PathContainer(List<PathData> paths) : base(paths)
        {
        }
    }

    public class EditorPathContainer : PathsContainer<PathEditorData>
    {
        public EditorPathContainer(List<PathEditorData> paths) : base(paths)
        {
        }

        public bool TryAddPath(string name)
        {
            var path = new PathEditorData(name);
            return Paths.TryAdd(name, path);
        }

        public void AddPath(PathData.SavePathData pathData)
        {
            var pathEditor = new PathEditorData(pathData);
            Paths.Add(pathData.Name, pathEditor);
        }

        public void Clear()
        {
            Paths.Clear();
        }

        public bool TryRemove(string name)
        {
            bool result = Paths.Remove(name, out var pathEditorData);
            pathEditorData.Dispose();
            return result;
        }

        public string GetLastName()
        {
            if(Paths.Keys.Count < 1)
                return String.Empty;
            
            return Paths.Keys.Last();
        }
    }
}