using System.Collections.Generic;
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

        public void AddMob(MobController mobController)
        {
            _mobs.Add(mobController.Id, mobController);
            AddMobByPosition(mobController.HexPosition.GetHashCode(), mobController);
        }

        private void AddMobByPosition(int positionHash, MobController mobController)
        {
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

        private void UpdateMobsLogicMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobs)
            {
                _mobsByPosition[mobPair.Value.GetHashCode()].Remove(mobPair.Value);
                mobPair.Value.LogicMove(frameLength);
                mobPair.Value.UpdateHexPosition(_hexPositionConversionService);
                AddMobByPosition(mobPair.Value.HexPosition.GetHashCode(), mobPair.Value);
            }
        }

        public void Clear()
        {
            _mobs.Clear();
            _mobsByPosition.Clear();
        }
    }
}