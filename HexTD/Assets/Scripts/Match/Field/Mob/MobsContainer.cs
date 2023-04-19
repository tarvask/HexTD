using System.Collections;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Shooting;

namespace Match.Field.Mob
{
    public class MobsContainer : ITypeTargetContainer
    {
        private readonly Dictionary<int, MobController> _mobs;
        private readonly Dictionary<int, List<ITargetable>> _mobsByPosition;

        public IReadOnlyDictionary<int, List<ITargetable>> TargetsByPosition => _mobsByPosition;
        public IReadOnlyDictionary<int, MobController> Mobs => _mobs;

        public MobsContainer()
        {
            _mobs = new Dictionary<int, MobController>();
            _mobsByPosition = new Dictionary<int, List<ITargetable>>();
        }

        public void AddMob(MobController mobController)
        {
            _mobs.Add(mobController.Id, mobController);
            mobController.SubscribeOnHexPositionChange(HandleMobHexPositionUpdate);
            AddMobByPosition(mobController);
        }

        private void HandleMobHexPositionUpdate(MobController mobController, Hex2d oldPosition)
        {
            _mobsByPosition[oldPosition.GetHashCode()].Remove(mobController);
            AddMobByPosition(mobController);
        }

        private void AddMobByPosition(MobController mobController)
        {
            int positionHash = mobController.HexPosition.GetHashCode();
            if (_mobsByPosition.ContainsKey(positionHash))
            {
                _mobsByPosition[positionHash].Add(mobController);
            }
            else
            {
                List<ITargetable> mobControllers = new List<ITargetable>();
                _mobsByPosition.Add(positionHash, mobControllers);
                mobControllers.Add(mobController);
            }
        }

        public void RemoveMob(MobController mobController)
        {
            _mobs.Remove(mobController.Id);
            RemoveByPosition(mobController);
            mobController.UnsubscribeOnHexPositionChange(HandleMobHexPositionUpdate);
        }

        private void RemoveByPosition(MobController mobController)
        {
            if(!_mobsByPosition.TryGetValue(mobController.HexPosition.GetHashCode(), out var mobsList))
                return;

            mobsList.Remove(mobController);
        }

        public void Clear()
        {
            _mobs.Clear();
            _mobsByPosition.Clear();
        }

        public IEnumerator<ITargetable> GetEnumerator()
        {
            foreach (var mobControllerPair in _mobs)
            {
                yield return mobControllerPair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}