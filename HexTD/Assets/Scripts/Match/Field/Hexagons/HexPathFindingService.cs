using System.Collections.Generic;
using HexSystem;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public class HexPathFindingService
    {
        public static readonly Hex2d[][] DiagonalsAdjacents =
        {
            new[]
            {
                new Hex2d(1, 0),
                new Hex2d(1, -1)
            },
            new[]
            {
                new Hex2d(0, -1),
                new Hex2d(1, -1)
            },
            new[]
            {
                new Hex2d(-1, 0),
                new Hex2d(0, -1)
            },
            new[]
            {
                new Hex2d(-1, 1),
                new Hex2d(-1, 0)
            },
            new[]
            {
                new Hex2d(0, 1),
                new Hex2d(-1, 1)
            },
            new[]
            {
                new Hex2d(1, 0),
                new Hex2d(0, 1)
            }
        };
        
        private readonly HexagonalFieldModel _hexagonalFieldModel;
        
        // прибавка к стоймости в случае неправильной высоты перехода с хекса на хекс 
        private readonly int _wrongHeightCostThreshold = 1000;

        private readonly PriorityQueue<Hex2d, int> _openSet =
            new PriorityQueue<Hex2d, int>();

        private readonly Dictionary<Hex2d, int> _knownCosts = new Dictionary<Hex2d, int>();
        private readonly Dictionary<Hex2d, Hex2d> _cameFrom = new Dictionary<Hex2d, Hex2d>();

        private int _cheapestFCost;
        private Hex2d _cheapestHex;

        public Vector3 HexSize => _hexagonalFieldModel.HexSize;

        public HexPathFindingService(HexagonalFieldModel hexagonalFieldModel)
        {
            _hexagonalFieldModel = hexagonalFieldModel;
        }

        public bool GetPath(List<Hex2d> resultPath, Hex2d targetHex, Hex2d startingHex, int searchRadius, out Hex2d cheapest,
            bool isIncludeStart = true)
        {
            _cheapestFCost = GetHeuristicDistance(startingHex, targetHex);
            _cheapestHex = startingHex;
            resultPath.Clear();
            _cameFrom.Clear();
            _openSet.Clear();
            _knownCosts.Clear();
            _openSet.Enqueue(startingHex, 0);
            _knownCosts[startingHex] = 0;
            _cameFrom.Add(startingHex, startingHex);

            int maxIterations = (6 * searchRadius * (searchRadius + 1) / 2) + 1;

            while (_openSet.Count > 0)
            {
                if (maxIterations <= 0)
                {
                    cheapest = _cheapestHex;
                    return false;
                }

                --maxIterations;

                _openSet.TryPeek(out var currentHex, out var currentCost);
                _openSet.Dequeue();

                if (currentHex.DistanceTo(startingHex) > searchRadius)
                {
                    continue;
                }

                if (currentHex.Equals(targetHex))
                {
                    break;
                }

                for (int i = 0; i < DiagonalsAdjacents.Length; ++i)
                {
                    for (int j = 0; j < DiagonalsAdjacents[i].Length; ++j)
                    {
                        var hexNeighbor = currentHex + DiagonalsAdjacents[i][j];

                        bool isNeighbourReachable =
                            _hexagonalFieldModel.IsHexInMap(hexNeighbor);
     
                        int neighbourCost = _knownCosts[currentHex] + 10;
                        if (isNeighbourReachable && 
                            (!_knownCosts.TryGetValue(hexNeighbor, out var cost3) || neighbourCost < cost3))
                        {
                            var fCost = neighbourCost + GetHeuristicDistance(hexNeighbor, targetHex);
                            TryUpdateCheapest(fCost, hexNeighbor);
                            _knownCosts[hexNeighbor] = neighbourCost;
                            _openSet.Enqueue(hexNeighbor, fCost);
                            _cameFrom[hexNeighbor] = currentHex;
                        }
                    }
                }
            }

            Hex2d current = targetHex;
            if (!_cameFrom.ContainsKey(targetHex))
            {
                cheapest = _cheapestHex;
                return false; // no path can be found
            }

            while (current != startingHex)
            {
                resultPath.Add(current);
                current = _cameFrom[current];
            }

            if (isIncludeStart)
            {
                resultPath.Add(startingHex);
            }

            resultPath.Reverse();

            cheapest = _cheapestHex;
            return true;
        }

        private static int GetHeuristicDistance(Hex2d targetHex, Hex2d hexDiagonal)
        {
            return (int)(hexDiagonal.DistanceTo(targetHex) * 100);
        }

        private void TryUpdateCheapest(int cost, Hex2d hex)
        {
            if (cost < _cheapestFCost)
            {
                _cheapestFCost = cost;
                _cheapestHex = hex;
            }
        }
    }
}