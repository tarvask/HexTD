using System.Collections.Generic;
using BuffLogic;
using ExitGames.Client.Photon;
using Match.Field.VFX;
using Tools;
using Tools.Interfaces;
using UI.ScreenSpaceOverlaySystem;
using UniRx;
using Zenject;

namespace Match.Field.Tower
{
    public class TowersManager : BaseDisposable, IOuterLogicUpdatable, ISerializableToNetwork
    {
        private readonly VfxManager _vfxManager;
        private readonly TowerContainer _towerContainer;
        private readonly List<TowerController> _dyingTowers;
        
        private readonly ScreenSpaceOverlayController _screenSpaceOverlayController;
        
        public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }

        public ITowerContainer TowerContainer => _towerContainer;
        // towers by ids
        public IReadOnlyDictionary<int, TowerController> Towers => _towerContainer.Towers;
        
        public TowersManager(VfxManager vfxManager, int fieldHexGridSize, 
            ScreenSpaceOverlayController screenSpaceOverlayController)
        {
            _vfxManager = vfxManager;
            _towerContainer = new TowerContainer(fieldHexGridSize);
            _dyingTowers = new List<TowerController>(fieldHexGridSize);
            _screenSpaceOverlayController = screenSpaceOverlayController;
            
            TowerBuiltReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerPreUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerRemovedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
        }

        public bool TryGetTowerInPositionHash(int positionHash, out TowerController towerInPosition)
        {
            return _towerContainer.TryGetTowerInPositionHash(positionHash, out towerInPosition);
        }

        public void AddTower(TowerController tower)
        {
            _towerContainer.AddTower(tower);
            TowerBuiltReactiveCommand.Execute(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            TowerPreUpgradedReactiveCommand.Execute(tower);
            tower.Upgrade();
            TowerUpgradedReactiveCommand.Execute(tower);
        }
        
        public void RemoveTower(TowerController removingTower)
        {
            _towerContainer.RemoveTower(removingTower);
            TowerRemovedReactiveCommand.Execute(removingTower);
            _vfxManager.ReleaseVfx(removingTower);
            _screenSpaceOverlayController.RemoveByTarget(removingTower);
            removingTower.Dispose();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var tower in _towerContainer.Towers.Values)
            {
                tower.OuterLogicUpdate(frameLength);

                if (tower.BaseReactiveModel.Health.Value.CurrentValue <= 0)
                    _dyingTowers.Add(tower);
            }

            foreach (TowerController dyingTower in _dyingTowers)
            {
                RemoveTower(dyingTower);
            }
            
            _dyingTowers.Clear();
        }

        public void Clear()
        {
            _towerContainer.Clear();
        }
        
        public Hashtable ToNetwork()
        {
            return _towerContainer.ToNetwork();
        }

        public class Factory : PlaceholderFactory<VfxManager, int, TowersManager>
        {
        }
    }
}