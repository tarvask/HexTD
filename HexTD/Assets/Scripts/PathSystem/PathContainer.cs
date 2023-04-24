using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Match.Field.Hexagons;

namespace PathSystem
{
    public class PathContainer : PathsContainer<PathData>
    {
        public PathContainer(HexPathFindingService hexPathFindingService, PathData.SavePathData[] savePaths)
        {
            foreach (var pathData in savePaths)
            {
                var path = new PathData(hexPathFindingService, pathData.PathId, pathData.Points);
                Paths.Add(pathData.PathId, path);
            }
        }
        
        public bool GetHexIsRoad(Hex2d hex2d) => Paths.Values.Any(pathData => pathData.GetHexIsRoad(hex2d));
    }
}