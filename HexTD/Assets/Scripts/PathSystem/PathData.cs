using System.Collections;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagons;
using Newtonsoft.Json;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
            
            public SavePathData(string name,
                List<Hex2d> points)
            {
                Name = name;
                Points = new LinkedList<Hex2d>(points);
            }
            
            public Hashtable ToNetwork()
            {
                Hashtable hexNetwork = new Hashtable{
                    {PhotonEventsConstants.SyncMatch.PathData.Name, Name},
                    {PhotonEventsConstants.SyncMatch.PathData.PointLength, (byte)Points.Count}
                };

                int i = 0;
                foreach (var point in Points)
                {
                    hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.PathData.PointQ}{i}", 
                        point.Q);
                    hexNetwork.Add($"{PhotonEventsConstants.SyncMatch.PathData.PointR}{i}", 
                        point.R);
                    i++;
                }

                return hexNetwork;
            }
        }

        public string Name { get; protected set; }

        protected readonly float PathLength;
        protected readonly LinkedList<Hex2d> Points;

        public PathData(string name, IEnumerable<Hex2d> points)
        {
            Name = name;
            Points = new LinkedList<Hex2d>(points);
            PathLength = CalcPathLength(Vector3.one);
        }

        public PathData(HexPathFindingService hexPathFindingService, string name, IEnumerable<Hex2d> points)
        {
            Name = name;
            Points = new LinkedList<Hex2d>();

            List<Hex2d> middlePath = new List<Hex2d>();
            var pointsEnumerator = points.GetEnumerator();
            pointsEnumerator.MoveNext();
            Hex2d prevPoint = pointsEnumerator.Current;
            Points.AddFirst(prevPoint);
            
            for(var lastNode = Points.Last; pointsEnumerator.MoveNext(); )
            {
                int length = (pointsEnumerator.Current - prevPoint).Length;
                if (length > 1)
                {
                    hexPathFindingService.GetPath(middlePath, pointsEnumerator.Current, prevPoint,
                        length, out var cheapestPoint, false);

                    var prevMiddlePoint = prevPoint;
                    foreach (var middlePoint in middlePath)
                    {
                        lastNode = FillLine(prevMiddlePoint, middlePoint, lastNode);
                        prevMiddlePoint = middlePoint;
                    }
                }
                else
                {
                    Points.AddLast(pointsEnumerator.Current);
                    lastNode = lastNode.Next;
                }
                
                prevPoint = pointsEnumerator.Current;
            }
            
            PathLength = CalcPathLength(hexPathFindingService.HexSize);
        }

        private LinkedListNode<Hex2d> FillLine(Hex2d firstPoint, Hex2d secondPoint, LinkedListNode<Hex2d> lastNode)
        {
            int length = (firstPoint - secondPoint).Length;
            if (length <= 1)
            {
                Points.AddAfter(lastNode, secondPoint);
                return lastNode.Next;
            }

            if (firstPoint.Q == secondPoint.Q)
            {
                int q = firstPoint.Q;
                int direction = (int)Mathf.Sign(secondPoint.R - firstPoint.R);
                for (int r = firstPoint.R; length > 0; length--)
                {
                    Points.AddAfter(lastNode, new Hex2d(q, r));
                    lastNode = lastNode.Next;
                    r += direction;
                }
            }

            else
            {
                int r = firstPoint.R;
                int direction = (int)Mathf.Sign(secondPoint.Q - firstPoint.Q);
                for (int q = firstPoint.Q; length > 0; length--)
                {
                    Points.AddAfter(lastNode, new Hex2d(q, r));
                    lastNode = lastNode.Next;
                    q += direction;
                }
            }

            return lastNode;
        }

        public SavePathData GetSavePathData()
        {
            return new SavePathData(Name, Points);
        }
        
        public IPathEnumerator GetPathEnumerator()
        {
            return new PathEnumerator(Points, PathLength);
        }
        
        public virtual IEnumerator<Hex2d> GetEnumerator()
        {
            return new PathEnumerator(Points, PathLength);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private float CalcPathLength(Vector3 hexSize)
        {
            float pathLength = 0f;
            float diagonalLength = Mathf.Sqrt(hexSize.x * hexSize.x + hexSize.z * hexSize.z);

            var curPoint = Points.First.Next;
            while (curPoint != null)
            {
                var delta = curPoint.Value - curPoint.Previous.Value;
                if (delta.Q == 0)
                    pathLength += hexSize.x;
                else
                    pathLength += diagonalLength;
                
                curPoint = curPoint.Next;
            }

            return pathLength;
        }
    }
}