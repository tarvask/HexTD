using HexSystem;
using Lean.Touch;
using MapEditor;
using Match.Commands;
using Match.Field.Hand;
using Match.Field.Tower.TowerConfigs;
using Services;
using System.Linq;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match.Field.Tower
{
    public class TowerPlacer : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public FieldConstructionProcessController ConstructionProcessController { get; }
            public PlayerHandController PlayerHandController { get; }
            public ConfigsRetriever ConfigsRetriever { get; }
            public FieldModel FieldModel { get; }
            public MatchCommands MatchCommands { get; }
            public ReactiveCommand<bool> DragCardChangeStatusCommand { get; }

            public Context(FieldConstructionProcessController constructionProcessController, PlayerHandController playerHandController, ConfigsRetriever configsRetriever, FieldModel fieldModel, MatchCommands matchCommands, ReactiveCommand<bool> dragCardChangeStatusCommand)
            {
                ConstructionProcessController = constructionProcessController;
                PlayerHandController = playerHandController;
                ConfigsRetriever = configsRetriever;
                FieldModel = fieldModel;
                MatchCommands = matchCommands;
                DragCardChangeStatusCommand = dragCardChangeStatusCommand;
            }
        }

        private readonly Context _context;

        public TowerPlacer(Context context)
        {
            _context = context;

            _context.DragCardChangeStatusCommand.Subscribe(DragCardChangeStatus);
            _context.MatchCommands.Incoming.ApplyBuildTower.Subscribe(ProcessBuild);
        }

        private TowerView _towerInstance;

        private Plane _plane = new Plane(Vector3.down, 0);

        private Hex2d _lastValidateHex;

        private Vector3 _offset = new Vector3(0, 2f, 0);

        private bool _activeDragProcess = false;
        private bool _canPlace = false;

        public void OuterLogicUpdate(float frameLength)
        {
            if (_activeDragProcess)
            {
                var finger = LeanTouch.Fingers.First();
                var ray = finger.GetRay(Camera.main);
                var hits = Physics.RaycastAll(ray, float.MaxValue);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.transform.parent.TryGetComponent<HexObject>(out var hex) == true)
                    {
                        if (_context.FieldModel.GetFieldHexType(hex.HitHex) == FieldHexType.Free)
                        {
                            _lastValidateHex = hex.HitHex;
                            _towerInstance.transform.position = hex.transform.position + _offset;
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
        }

        private void DragCardChangeStatus(bool isDraged)
        {
            if (isDraged)
            {
                StartDragProcess();
            }
            else
            {
                EndDragProcess();
            }
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

            if (_canPlace == true && _context.FieldModel.GetFieldHexType(_lastValidateHex) == FieldHexType.Free)
            {
                ProcessPreBuild(_lastValidateHex);
            }

            DestroyTowerInstance();
        }

        private TowerView CreateTowerView()
        {
            TowerShortParams towerParams = new TowerShortParams(_context.PlayerHandController.ChosenTowerType, 1);
            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerParams.TowerType);

            return _context.ConstructionProcessController.SetTowerView(towerConfig);
        }

        private void ProcessPreBuild(Hex2d clickedCell)
        {
            if (!_context.PlayerHandController.IsTowerChoice)
                return;

            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(
                _context.PlayerHandController.ChosenTowerType);

            if (_context.PlayerHandController.EnergyCharger.CurrentEnergyCount.Value >= towerConfig.TowerLevelConfigs[0].BuildPrice)
                _context.MatchCommands.Outgoing.RequestBuildTower.Fire(clickedCell,
                    new TowerShortParams(_context.PlayerHandController.ChosenTowerType, 1));
        }

        private void ProcessBuild(Hex2d position, TowerShortParams towerShortParams)
        {
            // check consistency
            if (!_context.FieldModel.IsHexWithType(position, FieldHexType.Free))
                return;

            TowerConfigNew towerConfig = _context.ConfigsRetriever.GetTowerByType(towerShortParams.TowerType);
            _context.PlayerHandController.UseChosenTower(towerConfig.TowerLevelConfigs[0].BuildPrice);
            _context.ConstructionProcessController.SetTowerBuilding(towerConfig, position);
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