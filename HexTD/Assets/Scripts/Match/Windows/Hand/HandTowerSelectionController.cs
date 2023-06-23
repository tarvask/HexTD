using System.Collections.Generic;
using Match.Field.Hand;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Services;
using UI.Tools;
using UniRx;
using BaseDisposable = Tools.BaseDisposable;

namespace Match.Windows.Hand
{
    public class HandTowerSelectionController : BaseDisposable
    {
        public struct Context
        {
            public HandTowerSelectionView View { get; }
            public PlayerHandController PlayerHandController { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public ReactiveCommand<bool> DragCardChangeStatusCommand { get; }

            public Context(HandTowerSelectionView view,
                PlayerHandController playerHandController,
                ConfigsRetriever configsRetriever, ReactiveCommand<bool> dragCardChangeStatusCommand)
            {
                View = view;
                PlayerHandController = playerHandController;
                ConfigsRetriever = configsRetriever;
                DragCardChangeStatusCommand = dragCardChangeStatusCommand;
            }
        }

        private readonly Context _context;
        private readonly UiElementListPool<TowerCardView> _towerCardViews;
        private readonly Dictionary<TowerType, TowerCardView> _towerCards;
        private readonly List<TowerCardController> _towerCardControllers = new List<TowerCardController>();

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

            TowerCardController.Context towerCardControllerContext = new TowerCardController.Context(
                towerCardView, _context.PlayerHandController, _context.DragCardChangeStatusCommand, towerType);
            TowerCardController towerCardController = new TowerCardController(towerCardControllerContext);
            _towerCardControllers.Add(towerCardController);

            int price = towerConfig.TowerLevelConfigs[TowerConfigNew.FirstTowerLevel].BuildPrice;
            towerCardView.CostText.text = price.ToString();
            towerCardView.TowerNameText.text = towerConfig.RegularParameters.TowerName + towerType.ToString();
            
            towerCardView.Button.onClick.RemoveAllListeners();
            towerCardView.Button.onClick.AddListener(() => 
                _context.PlayerHandController.SetChosenTower(towerType));

            towerCardView.ReadyTowerImage.sprite = towerConfig.Image;
            towerCardView.NotReadyTowerImage.sprite = towerConfig.Image;

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
            towerCardView.SetTowerCardReadyState(isReady);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            foreach (TowerCardController towerCardController in _towerCardControllers)
            {
                towerCardController.Dispose();
            }
            
            _towerCardViews.ClearList();
            _towerCards.Clear();
            _towerCardControllers.Clear();
        }
    }
}