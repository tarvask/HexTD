using System;
using System.Collections.Generic;
using Match.Field.Mob;
using Match.Field.Shooting.TargetFinding.Tactics;
using Tools;
using UnityEngine;

namespace Match.Field.Shooting.TargetFinding
{
    public class TargetFinder : BaseDisposable
    {
        private readonly MobsInRangeDefiner _mobsInRangeDefiner;
        private readonly MobsQualifier _mobsQualifier;
        private readonly Dictionary<byte, ITargetFindingTactic> _tactics;

        public TargetFinder()
        {
            _mobsInRangeDefiner = AddDisposable(new MobsInRangeDefiner());
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

        public int GetTargetWithTacticInRange(Dictionary<int, MobController> mobs, TargetFindingTacticType tacticType,
            Vector3 towerPosition, float attackRadiusSqr, bool preferUnbuffed/*, List<AbstractBuffParameters> towerActiveBuffs*/)
        {
            if (_tactics.TryGetValue((byte) tacticType, out ITargetFindingTactic tactic))
                return tactic.GetTargetWithTactic(
                    _mobsQualifier.GetMobsWithoutBuffs(_mobsInRangeDefiner.GetTargetsInRange(mobs, towerPosition, attackRadiusSqr),
                    preferUnbuffed));//, towerActiveBuffs));
            
            throw new ArgumentException("Unknown or undefined tactic type");
        }
    }
}