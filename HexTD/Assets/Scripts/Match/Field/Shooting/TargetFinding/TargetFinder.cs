using System;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Shooting.TargetFinding.Tactics;
using Match.Field.Tower;
using Tools;

namespace Match.Field.Shooting.TargetFinding
{
    public class TargetFinder : BaseDisposable
    {
        private readonly MobsInRangeDefiner _mobsInRangeDefiner;
        private readonly MobsQualifier _mobsQualifier;
        private readonly Dictionary<byte, ITargetFindingTactic> _tactics;

        public TargetFinder(HexMapReachableService hexMapReachableService)
        {
            _mobsInRangeDefiner = AddDisposable(new MobsInRangeDefiner(hexMapReachableService));
            _mobsQualifier = AddDisposable(new MobsQualifier());

            ITargetFindingTactic firstInLineTactic = AddDisposable(new FirstInLineTactic());
            ITargetFindingTactic randomTactic = AddDisposable(new RandomTactic());
            ITargetFindingTactic theToughestOneTactic = AddDisposable(new ToughestOneTactic());

            _tactics = new Dictionary<byte, ITargetFindingTactic>()
            {
                {(byte)firstInLineTactic.TacticType, firstInLineTactic},
                {(byte)randomTactic.TacticType, randomTactic},
                {(byte)theToughestOneTactic.TacticType, theToughestOneTactic}
            };
        }

        public int GetTargetWithTacticInRange(IReadOnlyDictionary<int, List<ITarget>> targetsByPosition, 
            ReachableAttackTargetFinderType reachableAttackTargetFinderType,
            TargetFindingTacticType tacticType,
            Hex2d towerPosition, int attackRadius, bool preferUnbuffed)
        {
            if (!_tactics.TryGetValue((byte) tacticType, out ITargetFindingTactic tactic))
                throw new ArgumentException("Unknown or undefined tactic type");

            var mobsInRange = _mobsInRangeDefiner.GetTargetsInRange(
                reachableAttackTargetFinderType,
                targetsByPosition, towerPosition, attackRadius);
            return tactic.GetTargetWithTactic(
                _mobsQualifier.GetMobsWithoutBuffs(mobsInRange, preferUnbuffed));
            
        }
    }
}