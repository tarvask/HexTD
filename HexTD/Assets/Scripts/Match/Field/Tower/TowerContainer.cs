using System.Collections;
using System.Collections.Generic;
using BuffLogic;
using Match.Field.Shooting;
using Match.Serialization;
using Services;

namespace Match.Field.Tower
{
    public interface ITowerContainer : ITypeTargetContainer, IShooterContainer
    {
        IReadOnlyDictionary<int, TowerController> Towers { get; }
    }
    
    public class TowerContainer : ITowerContainer
    {
        private readonly Dictionary<int, TowerController> _towers;
        private readonly Dictionary<int, List<ITarget>> _towersByPositions;

        public IReadOnlyDictionary<int, List<ITarget>> TargetsByPosition => _towersByPositions;
        public IReadOnlyDictionary<int, TowerController> Towers => _towers;

        public TowerContainer(int fieldHexGridSize)
        {
            _towers = new Dictionary<int, TowerController>(fieldHexGridSize);
            _towersByPositions = new Dictionary<int, List<ITarget>>(fieldHexGridSize);
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
                List<ITarget> mobControllers = new List<ITarget>();
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

        public bool TryGetTowerInPositionHash(int positionHash, out TowerController towerInPosition)
        {
            towerInPosition = null;
            
            if (!_towersByPositions.TryGetValue(positionHash, out var towersList))
                return false;

            if (towersList.Count == 0)
                return false;

            //TODO: remove casting
            towerInPosition = towersList[0] as TowerController;

            return towerInPosition != null;
        }

        public void Clear()
        {
            _towers.Clear();
            _towersByPositions.Clear();
        }

        public IEnumerator<ITarget> GetEnumerator()
        {
            foreach (var mobControllerPair in _towers)
            {
                yield return mobControllerPair.Value;
            }
        }

        IEnumerator<IShooter> IEnumerable<IShooter>.GetEnumerator()
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
        
        public ExitGames.Client.Photon.Hashtable ToNetwork()
        {
            return SerializerToNetwork.EnumerableToNetwork(Towers.Values, Towers.Count);
        }
        
        public static object FromNetwork(ExitGames.Client.Photon.Hashtable hashtable, ConfigsRetriever configsRetriever)
        {
            TowerContainer towerContainer = new TowerContainer(configsRetriever.TowerCount);
            towerContainer.Clear();
            
            foreach (var elementHashtable in SerializerToNetwork.IterateSerializedEnumerable(hashtable))
            {
                var tower = SerializerToNetwork.FromNetwork(elementHashtable) as TowerController;
                towerContainer.AddTower(tower);
            }

            return towerContainer;
        }
    }
}