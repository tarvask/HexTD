using System.Collections.Generic;
using HexSystem;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
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
            
            _constructingTowers = new Dictionary<int, TowerController>(_context.FieldModel.HexGridSize);
            _towersToRelease = new List<int>(_context.FieldModel.HexGridSize);
            
            _removingTowers = new Dictionary<int, TowerController>(_context.FieldModel.HexGridSize);
            _towersToDispose = new List<int>(_context.FieldModel.HexGridSize);
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
                if (!_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(towerPositionHash,
                        out TowerController releaseTower))
                    continue;
                
                _constructingTowers.Remove(towerPositionHash);
                releaseTower.Release();
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
                
                if (_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(towerPositionHash,
                    out TowerController towerInPosition))
                    _context.FieldModel.RemoveTower(towerPositionHash, towerInPosition);
            }
            
            _towersToDispose.Clear();
        }
        
        public void SetTowerRemoving(Hex2d position)
        {
            int hexHashCode = position.GetHashCode();

            if (!_context.FieldModel.TowersManager.TowerContainer.TryGetTowerInPositionHash(hexHashCode,
                    out TowerController removingTower))
                return;
            
            _removingTowers.Add(hexHashCode, removingTower);
            removingTower.SetRemoving();
        }

        public void SetTowerBuilding(TowerConfigNew towerConfig, Hex2d position)
        {
            int positionHash = position.GetHashCode();
            TowerController towerInstance = _context.Factory.CreateTower(towerConfig, position);
            towerInstance.SetLevel(1);
            _context.FieldModel.AddTower(towerInstance, position);
            _constructingTowers.Add(positionHash, towerInstance);
        }

        public void SetTowerUpgrading(TowerController tower)
        {
            int positionHash = tower.HexPosition.GetHashCode();
            _context.FieldModel.UpgradeTower(tower);
            _constructingTowers.Add(positionHash, tower);
        }

        public TowerController SetTowerInstance(TowerConfigNew towerConfig)
        {
            var hexToPlaceTower = _context.FieldModel.HexPositionConversionService.GetHexWithMinZ();
            return _context.Factory.CreateTowerWithId(towerConfig, hexToPlaceTower, -1, -1);
        }
    }
}