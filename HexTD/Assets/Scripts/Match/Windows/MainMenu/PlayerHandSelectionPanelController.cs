using System.Collections.Generic;
using Match.Field;
using Match.Field.Mob;
using Match.Field.Tower;
using Match.Windows.Tower;
using Services;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Windows.MainMenu
{
    public class PlayerHandSelectionPanelController : BaseDisposable
    {
        public struct Context
        {
            public PlayerHandSelectionPanelView View { get; }
            public FieldConfig FieldConfig { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public ReactiveProperty<byte> SelectedHandIndexReactiveProperty { get; }

            public Context(PlayerHandSelectionPanelView view, FieldConfig fieldConfig, ConfigsRetriever configsRetriever,
                ReactiveProperty<byte> selectedHandIndexReactiveProperty)
            {
                View = view;
                FieldConfig = fieldConfig;
                ConfigsRetriever = configsRetriever;
                SelectedHandIndexReactiveProperty = selectedHandIndexReactiveProperty;
            }
        }

        private readonly Context _context;

        private List<PlayerHandSelectionPossibleItemController> _towersInventory;

        private readonly IReactiveProperty<int> _openWindowsCountReactiveProperty;
        private readonly PlayerHandTowerSelectionWindowController _playerHandTowerSelectionWindowController;
        private readonly TowerInfoWindowController _towerInfoWindowController;

        private readonly ReactiveCommand<PlayerHandSelectionPossibleItemController> _selectPossibleTowerItemReactiveCommand;
        private readonly ReactiveCommand<TowerInHandChangeParameters> _towerInHandChangedReactiveCommand;

        private List<List<TowerConfig>> _hands;

        public PlayerHandSelectionPanelController(Context context)
        {
            _context = context;

            _openWindowsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(0));
            _towerInHandChangedReactiveCommand = AddDisposable(new ReactiveCommand<TowerInHandChangeParameters>());
            
            PlayerHandTowerSelectionWindowController.Context playerHandTowerSelectionWindowContext =
                new PlayerHandTowerSelectionWindowController.Context(_context.View.PlayerHandTowerSelectionWindowView, _towerInHandChangedReactiveCommand);
            _playerHandTowerSelectionWindowController = AddDisposable(new PlayerHandTowerSelectionWindowController(playerHandTowerSelectionWindowContext));
            TowerInfoWindowController.Context towerInfoWindowContext =
                new TowerInfoWindowController.Context(_context.View.TowerInfoWindowView, _openWindowsCountReactiveProperty);
            _towerInfoWindowController = AddDisposable(new TowerInfoWindowController(towerInfoWindowContext));
            _selectPossibleTowerItemReactiveCommand = AddDisposable(new ReactiveCommand<PlayerHandSelectionPossibleItemController>());

            CreateTowersInventory();

            // subscriptions
            for (byte handSwitchIndex = 0; handSwitchIndex < _context.View.HandsSwitchingButtons.Length; handSwitchIndex++)
            {
                byte index = handSwitchIndex;
                _context.View.HandsSwitchingButtons[handSwitchIndex].onClick.AddListener(() => SelectHand(index));
            }
            
            _selectPossibleTowerItemReactiveCommand.Subscribe(ShowTowerItemMenu);
            _context.View.TowerItemMenuCloseButton.onClick.AddListener(CloseTowerItemMenu);
            _towerInHandChangedReactiveCommand.Subscribe(ChangeTowerInHand);
        }
        
        public void Show(ref List<List<TowerConfig>> hands)
        {
            _hands = hands;
            SelectHand(_context.SelectedHandIndexReactiveProperty.Value, true);
            _context.View.gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _context.View.gameObject.SetActive(false);
            CloseTowerItemMenu();
            _playerHandTowerSelectionWindowController.Hide();
            _towerInfoWindowController.CloseWindow();
        }

        private void RefreshTowersInHand()
        {
            for (int towerIndex = 0; towerIndex < _hands[_context.SelectedHandIndexReactiveProperty.Value].Count; towerIndex++)
            {
                if (_hands[_context.SelectedHandIndexReactiveProperty.Value][towerIndex] != null)
                    _context.View.HandTowerItems[towerIndex].SetData(_hands[_context.SelectedHandIndexReactiveProperty.Value][towerIndex].Parameters.RegularParameters.Data);
                else
                    _context.View.HandTowerItems[towerIndex].SetData(null);
            }
        }

        private void CreateTowersInventory()
        {
            _towersInventory = new List<PlayerHandSelectionPossibleItemController>(_context.FieldConfig.TowersConfig.Towers.Count);
            
            foreach (TowerConfig towerConfig in _context.FieldConfig.TowersConfig.Towers)
            {
                PlayerHandSelectionPossibleItemView towerItemView =
                    Object.Instantiate(_context.View.PossibleTowerItemPrefab, GetElementRootByRace(towerConfig.Parameters.RegularParameters.Data.RaceType));
                
                PlayerHandSelectionPossibleItemController.Context towerItemControllerContext =
                    new PlayerHandSelectionPossibleItemController.Context(towerItemView, towerConfig, _selectPossibleTowerItemReactiveCommand);
                PlayerHandSelectionPossibleItemController towerItemController =
                    AddDisposable(new PlayerHandSelectionPossibleItemController(towerItemControllerContext));
                _towersInventory.Add(towerItemController);
            }
        }

        private void ShowTowerItemMenu(PlayerHandSelectionPossibleItemController possibleTowerItem)
        {
            // target scroll to item
            float scrollPositionY =
                ((Vector2) _context.View.TowerItemsScroll.transform.InverseTransformPoint(_context.View.TowerItemsRoot.position)).y
                - ((Vector2) _context.View.TowerItemsScroll.transform.InverseTransformPoint(possibleTowerItem.ItemRectTransform.position)).y;
            float scrollContentViewSizeDeltaY = _context.View.TowerItemsScroll.content.sizeDelta.y * 0.5f;
            float clampedScrollPositionY = Mathf.Clamp(scrollPositionY, -scrollContentViewSizeDeltaY, scrollContentViewSizeDeltaY);
            _context.View.TowerItemsRoot.anchoredPosition =
                new Vector2(_context.View.TowerItemsRoot.anchoredPosition.x, clampedScrollPositionY);

            // set up data
            _context.View.TowerItemMenu.TowerItem.SetData(possibleTowerItem.TowerConfig.Parameters.RegularParameters.Data);

            // set up position, do not use cached, because it has changed
            _context.View.TowerItemMenu.MenuRectTransform.position = possibleTowerItem.ItemRectTransform.position;

            // refresh buttons' subscription
            _context.View.TowerItemMenu.UseButton.onClick.RemoveAllListeners();
            _context.View.TowerItemMenu.UseButton.onClick.AddListener(() => ShowPlayerHandTowerSelectionWindow(possibleTowerItem.TowerConfig));
            _context.View.TowerItemMenu.InfoButton.onClick.RemoveAllListeners();
            _context.View.TowerItemMenu.InfoButton.onClick.AddListener(() => ShowTowerInfoWindow(possibleTowerItem.TowerConfig));
            
            _context.View.TowerItemMenu.gameObject.SetActive(true);
            _context.View.TowerItemMenuCloseButton.gameObject.SetActive(true);
        }

        private void CloseTowerItemMenu()
        {
            _context.View.TowerItemMenu.gameObject.SetActive(false);
            _context.View.TowerItemMenuCloseButton.gameObject.SetActive(false);
        }

        private void ShowTowerInfoWindow(TowerConfig towerConfig)
        {
            _towerInfoWindowController.ShowWindow(towerConfig.Parameters, 1,  /* null,*/ () => { });
        }

        private void ShowPlayerHandTowerSelectionWindow(TowerConfig selectedTowerConfig)
        {
            _playerHandTowerSelectionWindowController.Show(_hands[_context.SelectedHandIndexReactiveProperty.Value],
                _context.SelectedHandIndexReactiveProperty.Value, selectedTowerConfig);
        }

        private void ChangeTowerInHand(TowerInHandChangeParameters towerInHandChangeParameters)
        {
            _hands[towerInHandChangeParameters.HandNumber][towerInHandChangeParameters.SlotNumber] =
                _context.ConfigsRetriever.GetTowerByType(towerInHandChangeParameters.TowerType);
            RefreshTowersInHand();
        }

        private void SelectHand(byte handIndex, bool forceRefresh = false)
        {
            if (_context.SelectedHandIndexReactiveProperty.Value == handIndex && !forceRefresh)
                return;

            _context.SelectedHandIndexReactiveProperty.Value = handIndex;
            RefreshTowersInHand();
            
            for (int handSwitchIndex = 0; handSwitchIndex < _context.View.HandsSwitchingButtons.Length; handSwitchIndex++)
                _context.View.HandsSwitchingButtons[handSwitchIndex].interactable = (handSwitchIndex != handIndex);
        }

        private RectTransform GetElementRootByRace(RaceType elementRace)
        {
            switch (elementRace)
            {
                case RaceType.Water:
                    return _context.View.TowerWaterItemsRoot;
                case RaceType.Fire:
                    return _context.View.TowerFireItemsRoot;
                case RaceType.Nature:
                    return _context.View.TowerNatureItemsRoot;
                
                case RaceType.Earth:
                    return _context.View.TowerEarthItemsRoot;
                case RaceType.Death:
                    return _context.View.TowerDeathItemsRoot;
            }

            return null;
        }
    }
}