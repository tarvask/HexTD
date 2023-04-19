using System.Collections.Generic;
using Extensions;
using Match.Field;
using Match.Field.Hand;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Services;
using UI.Tools;
using UniRx;

namespace Match.Windows.Hand
{
    public class HandTowerSelectionController : BaseDisposable
    {
        public struct Context
        {
            public HandTowerSelectionView View { get; }
            public PlayerHandController PlayerHandController { get; }
            public ConfigsRetriever ConfigsRetriever { get; }

            public Context(HandTowerSelectionView view,
                PlayerHandController playerHandController,
                ConfigsRetriever configsRetriever)
            {
                View = view;
                PlayerHandController = playerHandController;
                ConfigsRetriever = configsRetriever;
            }
        }

        private readonly Context _context;
        private readonly UiElementListPool<TowerCardView> _towerCardViews;
        private readonly Dictionary<TowerType, TowerCardView> _towerCards;

        public HandTowerSelectionController(Context context)
        {
            _context = context;
            _towerCards = new Dictionary<TowerType, TowerCardView>(_context.ConfigsRetriever.TowerCount);
            _towerCardViews = AddDisposable(new UiElementListPool<TowerCardView>(_context.View.TowerCardView,
                _context.View.TowerCardRoot));
            _context.PlayerHandController.ReactiveTowers.ObserveAdd().Subscribe(addedTowerType => 
                CreateTowerCard(addedTowerType.Value));
            _context.PlayerHandController.ReactiveTowers.ObserveRemove().Subscribe(removedTowerType =>
                RemoveTowerCard(removedTowerType.Value));

            _context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Subscribe(
                count => _context.View.EnergyCountText.text = "Energy: " + count.ToString());

            foreach (var towerType in _context.PlayerHandController.Towers)
            {
                CreateTowerCard(towerType);
            }
        }

        private void CreateTowerCard(TowerType towerType)
        {
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerType);
            TowerCardView towerCardView = _towerCardViews.GetElement();
            _towerCards.Add(towerType, towerCardView);
            
            int price = towerConfig.TowerLevelConfigs[0].BuildPrice;
            towerCardView.CostText.text = price.ToString();
            towerCardView.TowerNameText.text = towerConfig.RegularParameters.TowerName + towerType.ToString();
            
            towerCardView.Button.onClick.RemoveAllListeners();
            towerCardView.Button.onClick.AddListener(() => 
                _context.PlayerHandController.SetChosenTower(towerType));

            _context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Subscribe(
                currentEnergyValue => SetTowerCardReadyState(towerCardView, price <= currentEnergyValue));
        }

        private void RemoveTowerCard(TowerType towerType)
        {
            if(_towerCards.Remove(towerType, out TowerCardView towerCardView))
                _towerCardViews.RemoveElement(towerCardView);
        }

        private void SetTowerCardReadyState(TowerCardView towerCardView, bool isReady)
        {
            towerCardView.ReadyBgImage.gameObject.SetActive(isReady);
            towerCardView.NotReadyBgImage.gameObject.SetActive(!isReady);
        }
    }
}