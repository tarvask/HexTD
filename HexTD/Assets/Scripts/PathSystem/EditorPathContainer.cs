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

        public bool TryAddPath(byte pathId)
        {
            var path = new PathEditorData(pathId);
            return Paths.TryAdd(pathId, path);
        }

        public void AddPath(PathData.SavePathData pathData)
        {
            var pathEditor = new PathEditorData(pathData);
            Paths.Add(pathData.PathId, pathEditor);
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

        public bool TryRemove(byte pathId)
        {
            bool result = Paths.Remove(pathId, out var pathEditorData);
            
            if (result)
                pathEditorData.Dispose();
            
            return result;
        }

        public void ChangeName(byte oldPathId, byte newPathId)
        {
            Paths.Remove(oldPathId, out var pathData);
            pathData.SetPathId(newPathId);
            Paths.Add(newPathId, pathData);
        }

        public void GetNames(List<byte> pathIds)
        {
            pathIds.Clear();
            foreach (var path in Paths)
            {
                pathIds.Add(path.Key);
            }
        }

        public byte GetFirstName()
        {
            if(Paths.Keys.Count < 1)
                return Byte.MaxValue;
            
            return Paths.Keys.Last();
        }

        public byte GetLastName()
        {
            if(Paths.Keys.Count < 1)
                return Byte.MaxValue;

            
            return Paths.Keys.Last();
        }
    }
}