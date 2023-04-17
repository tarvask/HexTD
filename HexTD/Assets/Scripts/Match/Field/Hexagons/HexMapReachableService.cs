using System;
using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public class HexMapReachableService
    {
        private readonly List<Hex2d> _reachableHexes;
        private readonly HexagonalFieldModel _hexagonalFieldModel;
        
        // Attack type / position / attack radius
        private readonly Dictionary<ReachableAttackTargetFinderType, Dictionary<int,Dictionary<int,IReadOnlyCollection<Hex2d>>>>
            _cachedAreas = new ();

        public HexMapReachableService(HexagonalFieldModel hexagonalFieldModel)
        {
            _hexagonalFieldModel = hexagonalFieldModel;
            _reachableHexes = new List<Hex2d>(hexagonalFieldModel.HexGridSize);
        }

        public IReadOnlyCollection<Hex2d> GetInRangeMapByTargetFinderType(
            Hex2d position,
            int attackRadius,
            ReachableAttackTargetFinderType reachableAttackTargetFinderType)
        {
            var positionHashCode = position.GetHashCode();
            var positionHexModel = _hexagonalFieldModel[positionHashCode];
            
            {
                if (_cachedAreas.TryGetValue(reachableAttackTargetFinderType, out var x))
                {
                    if (x.TryGetValue(positionHashCode, out var y))
                    {
                        if (y.TryGetValue(attackRadius, out var z))
                        {
                            return z;
                        }
                    }
                }
            }
            
            List<Hex2d> hexesInRadius = Hex2d.IterateSpiralRing(
                    position,
                    attackRadius +
                    (reachableAttackTargetFinderType == ReachableAttackTargetFinderType.HeightDependant
                        ? positionHexModel.Height
                        : 0))
                .ToList();
            
            List<Hex2d> obstacles = hexesInRadius.Where(hex =>
                    (_hexagonalFieldModel[hex.GetHashCode()]?.Height ?? -1)
                    > positionHexModel.Height)
                .ToList();

//            foreach (Hex2d obstacle in obstacles)
//            {
//                var x = _hexagonalFieldModel.GetPlanePosition(obstacle);
//                Debug.DrawLine(x,x+Vector3.up,Color.red,5.0f);
//            }

            IReadOnlyCollection<Hex2d> res;
            switch (reachableAttackTargetFinderType)
            {
                case ReachableAttackTargetFinderType.Simple:
                    res = GetConditionalInRangeMap(hexesInRadius, obstacles, position, attackRadius, SimpleCondition);
                    break;
                case ReachableAttackTargetFinderType.HeightDependant:
                    res = GetConditionalInRangeMap(hexesInRadius, obstacles, position, attackRadius, HeightDependantCondition);
                    break;
                case ReachableAttackTargetFinderType.Catapult:
                    res = GetConditionalInRangeMap(hexesInRadius, obstacles, position, attackRadius, CatapultCondition);
                    break;
                case ReachableAttackTargetFinderType.Horizontal:
                    res = GetConditionalInRangeMap(hexesInRadius, obstacles, position, attackRadius, HorizontalCondition);
                    break;
                default:
                    res = GetConditionalInRangeMap(hexesInRadius, obstacles, position, attackRadius, SimpleCondition);
                    break;
            }

            {
                if (!_cachedAreas.TryGetValue(reachableAttackTargetFinderType, out var x))
                {
                    x = new Dictionary<int, Dictionary<int, IReadOnlyCollection<Hex2d>>>();
                    _cachedAreas.Add(reachableAttackTargetFinderType, x);
                }

                if (!x.TryGetValue(positionHashCode, out var y))
                {
                    y = new Dictionary<int, IReadOnlyCollection<Hex2d>>();
                    x.Add(positionHashCode, y);
                }

                if (!y.ContainsKey(attackRadius))
                {
                    Debug.Log(111);
                    y.Add(attackRadius, res);
                }
            }

            return res;
        }

        private IReadOnlyCollection<Hex2d> GetConditionalInRangeMap(
            List<Hex2d> hexesInRadius, 
            List<Hex2d> obstacles,
            Hex2d position,
            int attackRadius,
            Func<List<Hex2d>, HexModel, HexModel, int, bool> condition)
        {
            _reachableHexes.Clear();

            var positionHexModel = _hexagonalFieldModel[position.GetHashCode()];

            foreach (var hex in hexesInRadius)
            {
                int hash = hex.GetHashCode();
                if(!_hexagonalFieldModel.IsHexInMap(hash))
                    continue;

                if (obstacles.Contains(hex))
                    continue;

                HexModel hexModel = _hexagonalFieldModel[hash];
                
                if(condition.Invoke(obstacles, positionHexModel, hexModel, attackRadius))
                    _reachableHexes.Add(hexModel.Position);
            }

            return _reachableHexes;
        }
        
        private bool SimpleCondition(List<Hex2d> obstacles, HexModel position, HexModel targetHexModel,
            int attackRadius)
        {
            Vector3 pos1 = _hexagonalFieldModel.GetPlanePosition(position.Position);
            Vector3 pos2 = _hexagonalFieldModel.GetPlanePosition(targetHexModel.Position);
            
            // shooter is higher, no obstacles
            return position.Height >= targetHexModel.Height
                && !obstacles.Any(obstacleHex => _hexagonalFieldModel.IsOnSegment(obstacleHex,
                    new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z)));
        }

        private bool HeightDependantCondition(List<Hex2d> obstacles, HexModel position, HexModel targetHexModel,
            int attackRadius)
        {
            Vector3 pos1 = _hexagonalFieldModel.GetPlanePosition(position.Position);
            Vector3 pos2 = _hexagonalFieldModel.GetPlanePosition(targetHexModel.Position);

//            Vector3 rand = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
//            Debug.DrawLine(pos1+rand,pos2+rand,Color.green,5.0f);
            
            // shooter is higher than target, height delta is added to attack radius, no obstacles
            return position.Height >= targetHexModel.Height
                   && (position.Position - targetHexModel.Position).Length <= attackRadius + position.Height - targetHexModel.Height
                   && !obstacles.Any(obstacleHex => _hexagonalFieldModel.IsOnSegment(obstacleHex,
                                                               new Vector2(pos1.x,pos1.z),new Vector2(pos2.x,pos2.z)));
        }

        private bool CatapultCondition(List<Hex2d> obstacles, HexModel position, HexModel targetHexModel, int attackRadius) =>
            true;

        private bool HorizontalCondition(List<Hex2d> obstacles, HexModel position, HexModel targetHexModel,
            int attackRadius)
        {
            Vector3 pos1 = _hexagonalFieldModel.GetPlanePosition(position.Position);
            Vector3 pos2 = _hexagonalFieldModel.GetPlanePosition(targetHexModel.Position);
            
            // shooter is at the same height, no obstacles
            return position.Height == targetHexModel.Height
                   && !obstacles.Any(obstacleHex => _hexagonalFieldModel.IsOnSegment(obstacleHex,
                       new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z)));
        }
    }
}