using System.Collections;
using System.Collections.Generic;
using HexSystem;
using Newtonsoft.Json;

namespace PathSystem
{
    public class PathData : IEnumerable<Hex2d>
    {
        public struct SavePathData
        {
            [JsonProperty("Name")] public string Name;
            [JsonProperty("Points")] public LinkedList<Hex2d> Points;
            
            [JsonConstructor]
            public SavePathData([JsonProperty("Name")] string name,
                [JsonProperty("Points")] LinkedList<Hex2d> points)
            {
                Name = name;
                Points = new LinkedList<Hex2d>(points);
            }
        }
        
        [JsonProperty("Name")] public string Name { get; }
        [JsonProperty("Points")] protected readonly LinkedList<Hex2d> Points;

        [JsonConstructor]
        public PathData([JsonProperty("Name")] string name,
            [JsonProperty("Points")] LinkedList<Hex2d> points)
        {
            Name = name;
            Points = new LinkedList<Hex2d>(points);
        }

        public PathData(PathData pathData)
        {
            Name = pathData.Name;
            Points = new LinkedList<Hex2d>(pathData.Points);
        }
        
        public PathData(string name)
        {
            Name = name;
            Points = new LinkedList<Hex2d>();
        }

        public PathData(string name, IEnumerable<Hex2d> points)
        {
            Name = name;
            Points = new LinkedList<Hex2d>(points);
        }

        public bool TryGetNextNode(Hex2d currentHex, out Hex2d nextHex)
        {
            var currentHexNode = Points.Find(currentHex);
            nextHex = currentHexNode?.Next?.Value ?? new Hex2d();
            return currentHexNode != null && currentHexNode.Next != null;
        }

        public SavePathData GetSavePathData()
        {
            return new SavePathData(Name, Points);
        }

        public virtual IEnumerator<Hex2d> GetEnumerator()
        {
            return new PathEnumerator(Points);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}