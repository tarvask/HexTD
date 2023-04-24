using System;
using System.Collections.Generic;
using System.Linq;

namespace PathSystem
{
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

        public List<PathData.SavePathData> GetPathsForSave()
        {
            var savePathDatas = new List<PathData.SavePathData>();
            foreach (var path in Paths.Values)
            {
                savePathDatas.Add(path.GetSavePathData());
            }
            
            return savePathDatas;
        }

        public void Clear()
        {
            Paths.Clear();
        }

        public bool TryRemove(string name)
        {
            bool result = Paths.Remove(name, out var pathEditorData);
            
            if (result)
                pathEditorData.Dispose();
            
            return result;
        }

        public void ChangeName(string oldName, string newName)
        {
            Paths.Remove(oldName, out var pathData);
            pathData.SetName(newName);
            Paths.Add(newName, pathData);
        }

        public void GetNames(List<string> names)
        {
            names.Clear();
            foreach (var path in Paths)
            {
                names.Add(path.Key);
            }
        }

        public string GetFirstName()
        {
            if(Paths.Keys.Count < 1)
                return String.Empty;
            
            return Paths.Keys.Last();
        }

        public string GetLastName()
        {
            if(Paths.Keys.Count < 1)
                return String.Empty;
            
            return Paths.Keys.Last();
        }
    }
}