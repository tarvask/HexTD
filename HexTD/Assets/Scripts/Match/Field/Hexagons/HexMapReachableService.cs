using System;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Tower;

namespace Match.Field.Hexagons
{
    public class HexMapReachableService
    {
        private readonly List<Hex2d> _reachableHexes;
        private readonly HexagonalFieldModel _hexagonalFieldModel;

        public HexMapReachableService(HexagonalFieldModel hexagonalFieldModel)
        {
            _hexagonalFieldModel = hexagonalFieldModel;
            _reachableHexes = new List<Hex2d>(hexagonalFieldModel.HexGridSize);
        }

        public IReadOnlyCollection<Hex2d> GeInRangeMapByTargetFinderType(
            Hex2d position,
            int attackRadius,
            ReachableAttackTargetFinderType reachableAttackTargetFinderType)
        {
            switch (reachableAttackTargetFinderType)
            {
                case ReachableAttackTargetFinderType.Simple:
                    return GetConditionalInRangeMap(position, attackRadius, SimpleCondition);

                case ReachableAttackTargetFinderType.Catapult:
                    return GetConditionalInRangeMap(position, attackRadius, CatapultCondition);
                
                case ReachableAttackTargetFinderType.Horizontal:
                    return GetConditionalInRangeMap(position, attackRadius, HorizontalCondition);

                case ReachableAttackTargetFinderType.HeightDependant:
                    return GetConditionalInRangeMap(position, attackRadius + 
                        _hexagonalFieldModel[position.GetHashCode()].Height - 1, HeightDependantCondition);

                default:
                    return GetConditionalInRangeMap(position, attackRadius, SimpleCondition);
            }
        }

        private IReadOnlyCollection<Hex2d> GetConditionalInRangeMap(Hex2d position, 
            int radius, Func<HexModel, HexModel, bool> condition)
        {
            _reachableHexes.Clear();

            var positionHexModel = _hexagonalFieldModel[position.GetHashCode()];
            
            IEnumerable<Hex2d> mapEnumerator = Hex2d.IterateSpiralRing(position, radius);
            foreach (var hex in mapEnumerator)
            {
                int hash = hex.GetHashCode();
                if(!_hexagonalFieldModel.IsHexInMap(hash))
                    continue;

                HexModel hexModel = _hexagonalFieldModel[hash];
                
                if(condition.Invoke(positionHexModel, hexModel))
                    _reachableHexes.Add(hexModel.Position);
            }

            return _reachableHexes;
        }

        //TODO: added condition on hexes by hex
        public bool SimpleCondition(HexModel position, HexModel targetHexModel) =>
            position.Height >= targetHexModel.Height;

        public bool CatapultCondition(HexModel position, HexModel targetHexModel) =>
            true;

        public bool HorizontalCondition(HexModel position, HexModel targetHexModel) =>
            position.Height == targetHexModel.Height;

        public bool HeightDependantCondition(HexModel position, HexModel targetHexModel) =>
            position.Height >= targetHexModel.Height;
    }
}