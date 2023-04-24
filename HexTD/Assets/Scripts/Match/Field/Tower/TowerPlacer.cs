using System;
using HexSystem;
using Lean.Touch;
using MapEditor;
using Match.Field.Hand;
using Match.Field.Tower.TowerConfigs;
using Services;
using System.Linq;
using Match.Field.Hexagons;
using PathSystem;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field.Tower
{
    public class TowerPlacer : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public FieldConstructionProcessController ConstructionProcessController { get; }
            public PlayerHandController PlayerHandController { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            
            public HexagonalFieldModel HexagonalFieldModel { get; }
            public PathContainer PathContainer { get; }
            
            public ReactiveCommand<bool> DragCardChangeStatusCommand { get; }
            public ReactiveCommand<Hex2d> PlaceForTowerSelectedCommand { get; }

            public Context(FieldConstructionProcessController constructionProcessController,
                PlayerHandController playerHandController,
                ConfigsRetriever configsRetriever,
                HexagonalFieldModel hexagonalFieldModel,
                PathContainer pathContainer,
                
                ReactiveCommand<bool> dragCardChangeStatusCommand,
                ReactiveCommand<Hex2d> placeForTowerSelectedCommand)
            {
                ConstructionProcessController = constructionProcessController;
                PlayerHandController = playerHandController;
                ConfigsRetriever = configsRetriever;
                HexagonalFieldModel = hexagonalFieldModel;
                PathContainer = pathContainer;
                DragCardChangeStatusCommand = dragCardChangeStatusCommand;
                PlaceForTowerSelectedCommand = placeForTowerSelectedCommand;
            }
        }

        private readonly Context _context;

        public TowerPlacer(Context context)
        {
            _context = context;

            _context.DragCardChangeStatusCommand.Subscribe(DragCardChangeStatus);
        }

        private TowerView _towerInstance;

        private Plane _plane = new Plane(Vector3.down, 0);

        private Hex2d _lastValidHex;
        private TowerConfigNew _currentTowerConfig;

        private bool _activeDragProcess;
        private bool _canPlace;

        public void OuterLogicUpdate(float frameLength)
        {
            if (_activeDragProcess)
            {
                UpdateDragProcess();
            }
        }

        public bool CanTowerBePlacedToHex(TowerConfigNew towerConfig, Hex2d clickedHex)
        {
            if (_context.HexagonalFieldModel.CurrentFieldHexTypes.GetFieldHexType(clickedHex) != FieldHexType.Free)
                return false;

            bool isBlocker = _context.HexagonalFieldModel.GetHexIsBlocker(clickedHex);
            if (isBlocker)
                return false;

            var isRoad = _context.PathContainer.GetHexIsRoad(clickedHex);
            switch (towerConfig.RegularParameters.PlacementType)
            {
                case TowerPlacementType.Anywhere:
                    return true;
                case TowerPlacementType.NotRoad:
                    return !isRoad;
                case TowerPlacementType.OnRoad:
                    return isRoad;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateDragProcess()
        {
            var finger = LeanTouch.Fingers.First();
            var ray = finger.GetRay(Camera.main);
            var hits = Physics.RaycastAll(ray, float.MaxValue);

            foreach (var hit in hits)
            {
                if (hit.collider.transform.parent.TryGetComponent<HexObject>(out var hex))
                {
                    if (CanTowerBePlacedToHex(_currentTowerConfig, hex.HitHex))
                    {
                        _lastValidHex = hex.HitHex;
                        _towerInstance.transform.position = hex.transform.position;
                        _canPlace = true;
                        return;
                    }
                }
            }

            _canPlace = false;
            if (_plane.Raycast(ray, out float distance))
            {
                _towerInstance.transform.position = ray.GetPoint(distance);
            }
        }

        private void DragCardChangeStatus(bool isDragged)
        {
            if (isDragged)
                StartDragProcess();
            else
                EndDragProcess();
        }

        private void StartDragProcess()
        {
            _activeDragProcess = true;
            DestroyTowerInstance();

            _towerInstance = CreateTowerView();

            var color = _towerInstance.GetComponentInChildren<MeshRenderer>().material.color;
            color.a = 0.5f;
            _towerInstance.GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        private void EndDragProcess()
        {
            _activeDragProcess = false;

            if (_canPlace && CanTowerBePlacedToHex(_currentTowerConfig, _lastValidHex))
            {
                _context.PlaceForTowerSelectedCommand.Execute(_lastValidHex);
            }

            DestroyTowerInstance();
        }

        private TowerView CreateTowerView()
        {
            TowerShortParams towerParams = new TowerShortParams(_context.PlayerHandController.ChosenTowerType, 1);
            _currentTowerConfig = _context.ConfigsRetriever.GetTowerByType(towerParams.TowerType);

            return _context.ConstructionProcessController.SetTowerView(_currentTowerConfig);
        }

        private void DestroyTowerInstance()
        {
            if (_towerInstance != null)
            {
                Object.Destroy(_towerInstance.gameObject);
            }
        }
    }
}