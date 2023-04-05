using System.Collections.Generic;
using Match.Field.Tower;
using Tools;
using Tools.Interfaces;
using UnityEngine;

namespace Match.Field
{
    public class FieldConstructionProcessController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public FieldFactory Factory { get; }

            public Context(FieldModel fieldModel, FieldFactory factory)
            {
                FieldModel = fieldModel;
                Factory = factory;
            }
        }

        private readonly Context _context;
        // constructing towers, are needed to control delayed building/upgrading
        private readonly Dictionary<int, TowerController> _constructingTowers;
        private readonly List<int> _towersToRelease;
        // removing towers, are needed to control delayed destroy
        private readonly Dictionary<int, TowerController> _removingTowers;
        private readonly List<int> _towersToDispose;

        public FieldConstructionProcessController(Context context)
        {
            _context = context;
            
            _constructingTowers = new Dictionary<int, TowerController>(_context.FieldModel.PlayableCells.Count);
            _towersToRelease = new List<int>(_context.FieldModel.PlayableCells.Count);
            
            _removingTowers = new Dictionary<int, TowerController>(_context.FieldModel.PlayableCells.Count);
            _towersToDispose = new List<int>(_context.FieldModel.PlayableCells.Count);
        }
        
        public void OuterLogicUpdate(float frameLength)
        { 
            // constructing towers
            foreach (KeyValuePair<int, TowerController> towerPair in _constructingTowers)
            {
                if (towerPair.Value.IsReadyToRelease)
                    _towersToRelease.Add(towerPair.Key);
            }

            foreach (int towerPositionHash in _towersToRelease)
            {
                _constructingTowers.Remove(towerPositionHash);
                _context.FieldModel.TowersByPositions[towerPositionHash].Release();
            }
            
            _towersToRelease.Clear();
            
            // removing towers
            foreach (KeyValuePair<int, TowerController> towerPair in _removingTowers)
            {
                if (towerPair.Value.IsReadyToDispose)
                    _towersToDispose.Add(towerPair.Key);
            }

            foreach (int towerPositionHash in _towersToDispose)
            {
                _removingTowers.Remove(towerPositionHash);
                _context.FieldModel.RemoveTower(towerPositionHash, _context.FieldModel.TowersByPositions[towerPositionHash]);
            }
            
            _towersToDispose.Clear();
        }
        
        public void SetTowerRemoving(Vector2Int position)
        {
            int positionHash = position.GetHashCode(_context.FieldModel.FieldWidth);
            TowerController removingTower = _context.FieldModel.TowersByPositions[positionHash];
            _removingTowers.Add(positionHash, removingTower);
            removingTower.SetRemoving();
        }

        public TowerController SetTowerBuilding(TowerConfig towerConfig, Vector2Int position)
        {
            int positionHash = position.GetHashCode(_context.FieldModel.FieldWidth);
            TowerController towerInstance = _context.Factory.CreateTower(towerConfig, position.x, position.y,
                _context.FieldModel.IsCellNearRoad(position));
            towerInstance.SetLevel(1);
            _context.FieldModel.AddTower(towerInstance, position);
            _constructingTowers.Add(positionHash, towerInstance);

            return towerInstance;
        }

        public void SetTowerUpgrading(TowerController tower)
        {
            int positionHash = new Vector2Int(tower.CellPositionX, tower.CellPositionY).GetHashCode(_context.FieldModel.FieldWidth);
            _context.FieldModel.UpgradeTower(tower);
            _constructingTowers.Add(positionHash, tower);
        }
    }
}