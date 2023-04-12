using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagons;

namespace Match.Field.Mob
{
    public class MobsContainer
    {
        private readonly IHexPositionConversionService _hexPositionConversionService;
        private readonly Dictionary<int, MobController> _mobs;
        private readonly Dictionary<int, List<MobController>> _mobsByPosition;

        public IReadOnlyDictionary<int, MobController> Mobs => _mobs;
        public IReadOnlyDictionary<int, List<MobController>> MobsByPosition => _mobsByPosition;

        public MobsContainer(IHexPositionConversionService hexPositionConversionService)
        {
            _hexPositionConversionService = hexPositionConversionService;
            _mobs = new Dictionary<int, MobController>();
            _mobsByPosition = new Dictionary<int, List<MobController>>();
        }

        public void UpdateMobsLogicMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobs)
            {
                mobPair.Value.LogicMove(frameLength);
            }
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
                List<MobController> mobControllers = new List<MobController>();
                _mobsByPosition.Add(positionHash, mobControllers);
                mobControllers.Add(mobController);
            }
        }

        public void RemoveMob(MobController mobController)
        {
            _mobs.Remove(mobController.Id);
            _mobsByPosition.Remove(mobController.HexPosition.GetHashCode());
        }

        public void Clear()
        {
            _mobs.Clear();
            _mobsByPosition.Clear();
        }
    }
}