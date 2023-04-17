using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Tower;
using Match.Wave;
using Tools;

namespace Match.Field.Shooting.TargetFinding
{
    public class MobsInRangeDefiner : BaseDisposable
    {
        private readonly HexMapReachableService _hexMapReachableService;
        private readonly List<ITargetable> _mobsInRange;

        public MobsInRangeDefiner(HexMapReachableService hexMapReachableService)
        {
            _hexMapReachableService = hexMapReachableService;
            _mobsInRange = new List<ITargetable>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public IReadOnlyList<ITargetable> GetTargetsInRange(
            ReachableAttackTargetFinderType reachableAttackTargetFinderType,
            IReadOnlyDictionary<int, List<ITargetable>> mobsByPosition,
            Hex2d towerPosition, int attackRadius)
        {
            IReadOnlyCollection<Hex2d> hexes = _hexMapReachableService.GeInRangeMapByTargetFinderType(
                towerPosition, attackRadius, reachableAttackTargetFinderType);
            _mobsInRange.Clear();
            
            foreach (Hex2d hex in hexes)
            {
                if (!mobsByPosition.TryGetValue(hex.GetHashCode(), out var mobsInPosition))
                    continue;
                
                foreach (var mobController in mobsInPosition)
                    _mobsInRange.Add(mobController);
            }

            return _mobsInRange;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _mobsInRange.Clear();
        }
    }
}