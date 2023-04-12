using System.Collections.Generic;

namespace PathSystem
{
    public class PathContainer : PathsContainer<PathData>
    {
        public PathContainer(List<PathData> paths) : base(paths)
        {
        }
        
        public PathContainer(PathData.SavePathData[] savePaths)
        {
            foreach (var pathData in savePaths)
            {
                var path = new PathData(pathData.Name, pathData.Points);
                Paths.Add(pathData.Name, path);
            }
        }
    }
}