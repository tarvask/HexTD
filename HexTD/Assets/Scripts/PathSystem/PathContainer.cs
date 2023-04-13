using System.Collections.Generic;
using Match.Field.Hexagons;

namespace PathSystem
{
    public class PathContainer : PathsContainer<PathData>
    {
        public PathContainer(List<PathData> paths) : base(paths)
        {
        }
        
        public PathContainer(HexPathFindingService hexPathFindingService, PathData.SavePathData[] savePaths)
        {
            foreach (var pathData in savePaths)
            {
                var path = new PathData(hexPathFindingService, pathData.Name, pathData.Points);
                Paths.Add(pathData.Name, path);
            }
        }
    }
}