using System.Collections.Generic;
using Match.Field.Tower;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldTowersAreaBuffsManager : BaseDisposable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            
            public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }

            public Context(FieldModel fieldModel,
                ReactiveCommand<TowerController> towerBuiltReactiveCommand,
                ReactiveCommand<TowerController> towerPreUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerRemovedReactiveCommand)
            {
                FieldModel = fieldModel;

                TowerBuiltReactiveCommand = towerBuiltReactiveCommand;
                TowerPreUpgradedReactiveCommand = towerPreUpgradedReactiveCommand;
                TowerUpgradedReactiveCommand = towerUpgradedReactiveCommand;
                TowerRemovedReactiveCommand = towerRemovedReactiveCommand;
            }
        }

        private readonly Context _context;

        public FieldTowersAreaBuffsManager(Context context)
        {
            _context = context;

            _context.TowerBuiltReactiveCommand.Subscribe(TowerBuiltEventHandler);
            _context.TowerPreUpgradedReactiveCommand.Subscribe(TowerPreUpgradedEventHandler);
            _context.TowerUpgradedReactiveCommand.Subscribe(TowerUpgradedEventHandler);
            _context.TowerRemovedReactiveCommand.Subscribe(TowerRemovedEventHandler);
        }

        private void TowerBuiltEventHandler(TowerController newTower)
        {
            ApplyBuffsToTowersInArea(newTower);
            ApplyBuffsFromTowersInArea(newTower);
        }

        private void TowerPreUpgradedEventHandler(TowerController preUpgradedTower)
        {
            // remove old
            RemoveBuffsFromTowersInArea(preUpgradedTower);
        }

        private void TowerUpgradedEventHandler(TowerController upgradedTower)
        {
            // add new
            ApplyBuffsToTowersInArea(upgradedTower);
            ApplyBuffsFromTowersInArea(upgradedTower);
        }
        
        private void TowerRemovedEventHandler(TowerController removedTower)
        {
            RemoveBuffsFromTowersInArea(removedTower);
        }

        private void ApplyBuffsToTowersInArea(TowerController castingTower)
        {
            if (castingTower.BuffsForNeighbouringTowers.Count <= 0)
                return;
         
            Vector2Int castingTowerPosition = new Vector2Int(castingTower.CellPositionX, castingTower.CellPositionY);
            
            // apply buffs from casting tower to others
            // iterate through all towers
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
            {
                if (towerPair.Key == castingTower.Id)
                    continue;
                
                Vector2Int currentTowerCellPosition =
                    new Vector2Int(towerPair.Value.CellPositionX, towerPair.Value.CellPositionY);
                float distanceBetweenTowers = (castingTowerPosition - currentTowerCellPosition).magnitude;

                // iterate through all area buffs and check if buff can be applied
                // foreach (AbstractBuffParameters towersInAreaBuff in castingTower.BuffsForNeighbouringTowers)
                // {
                //     if (towersInAreaBuff is ITowersInAreaApplicable towersInAreaApplicableBuff
                //         && distanceBetweenTowers <= towersInAreaApplicableBuff.EffectAreaRadius)
                //     {
                //         towerPair.Value.AddBuff(towersInAreaBuff);
                //     }
                // }
            }
        }

        private void ApplyBuffsFromTowersInArea(TowerController receivingTower)
        {
            Vector2Int receivingTowerPosition = new Vector2Int(receivingTower.CellPositionX, receivingTower.CellPositionY);
            
            // apply buffs from others to new tower
            // iterate through all towers
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
            {
                if (towerPair.Value.BuffsForNeighbouringTowers.Count <= 0)
                    continue;
                
                Vector2Int currentTowerCellPosition =
                    new Vector2Int(towerPair.Value.CellPositionX, towerPair.Value.CellPositionY);
                float distanceBetweenTowers = (receivingTowerPosition - currentTowerCellPosition).magnitude;

                // iterate through all area buffs and check if buff can be applied to new tower
                // foreach (AbstractBuffParameters towersInAreaBuff in towerPair.Value.BuffsForNeighbouringTowers)
                // {
                //     if (towersInAreaBuff is ITowersInAreaApplicable towersInAreaApplicableBuff
                //         && distanceBetweenTowers <= towersInAreaApplicableBuff.EffectAreaRadius)
                //     {
                //         receivingTower.AddBuff(towersInAreaBuff);
                //     }
                // }
            }
        }

        private void RemoveBuffsFromTowersInArea(TowerController castingTower)
        {
            if (castingTower.BuffsForNeighbouringTowers.Count <= 0)
                return;
            
            Vector2Int castingTowerPosition = new Vector2Int(castingTower.CellPositionX, castingTower.CellPositionY);
                
            // iterate through all towers
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
            {
                Vector2Int currentTowerCellPosition = new Vector2Int(towerPair.Value.CellPositionX, towerPair.Value.CellPositionY);
                float distanceBetweenTowers = (castingTowerPosition - currentTowerCellPosition).magnitude;

                // iterate through all area buffs and check if buff can be applied
                // foreach (AbstractBuffParameters towersInAreaBuff in castingTower.BuffsForNeighbouringTowers)
                // {
                //     if (towersInAreaBuff is ITowersInAreaApplicable towersInAreaApplicableBuff
                //         && distanceBetweenTowers <= towersInAreaApplicableBuff.EffectAreaRadius)
                //     {
                //         towerPair.Value.RemoveBuff(towersInAreaBuff.BuffedParameterType, towersInAreaBuff.BuffSubType);
                //     }
                // }
            }
        }
    }
}