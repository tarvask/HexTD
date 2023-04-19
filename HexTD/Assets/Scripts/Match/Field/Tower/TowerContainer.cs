using System.Collections;
using System.Collections.Generic;
using Match.Field.Shooting;

namespace Match.Field.Tower
{
    public interface ITowerContainer : ITypeTargetContainer, IShooterContainer
    {
        IReadOnlyDictionary<int, TowerController> Towers { get; }
        TowerController GetTowerByPositionHash(int positionHash);
    }
    
    public class TowerContainer : ITowerContainer
    {
        private readonly Dictionary<int, TowerController> _towers;
        private readonly Dictionary<int, List<ITargetable>> _towersByPositions;

        public IReadOnlyDictionary<int, List<ITargetable>> TargetsByPosition => _towersByPositions;
        public IReadOnlyDictionary<int, TowerController> Towers => _towers;

        public TowerContainer(int fieldHexGridSize)
        {
            _towers = new Dictionary<int, TowerController>(fieldHexGridSize);
            _towersByPositions = new Dictionary<int, List<ITargetable>>(fieldHexGridSize);
        }

        public void AddTower(TowerController tower)
        {
            _towers.Add(tower.Id, tower);
            AddTowerByPosition(tower);
        }

        private void AddTowerByPosition(TowerController towerController)
        {
            int positionHash = towerController.HexPosition.GetHashCode();
            if (_towersByPositions.ContainsKey(positionHash))
            {
                _towersByPositions[positionHash].Add(towerController);
            }
            else
            {
                List<ITargetable> mobControllers = new List<ITargetable>();
                _towersByPositions.Add(positionHash, mobControllers);
                mobControllers.Add(towerController);
            }
        }

        public void RemoveTower(TowerController towerController)
        {
            _towers.Remove(towerController.Id);
            RemoveByPosition(towerController);
        }

        private void RemoveByPosition(TowerController towerController)
        {
            if(!_towersByPositions.TryGetValue(towerController.HexPosition.GetHashCode(), out var towerList))
                return;

            towerList.Remove(towerController);
        }
        
        //TODO: remove casting
        public TowerController GetTowerByPositionHash(int positionHash)
        {
            return (TowerController)_towersByPositions[positionHash][0];
        }

        public void Clear()
        {
            _towers.Clear();
            _towersByPositions.Clear();
        }

        public IEnumerator<ITargetable> GetEnumerator()
        {
            foreach (var mobControllerPair in _towers)
            {
                yield return mobControllerPair.Value;
            }
        }

        IEnumerator<IShootable> IEnumerable<IShootable>.GetEnumerator()
        {
            foreach (var mobControllerPair in _towers)
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