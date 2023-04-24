using System;
using System.Collections.Generic;
using HexSystem;
using MapEditor;
using UI.Tools;
using UnityEngine;

namespace UI.PathEditorPanel
{
    public class PathPointListEditorViewController : BaseDisposable
    {
        private readonly PathEditInfoView _pathEditInfoView;
        private readonly PathEditorController _pathEditorController;
        private readonly UiElementListPool<PointEditorPanelView> _pointEditorPanelViews;

        private string _name;

        public PathEditInfoView PathEditInfoView => _pathEditInfoView;
        public string DisplayName => _name;

        public PathPointListEditorViewController(PathEditInfoView pathEditInfoView,
            PathEditorController pathEditorController,
            string name)
        {
            _pathEditInfoView = pathEditInfoView;
            _pointEditorPanelViews = AddDisposable(new UiElementListPool<PointEditorPanelView>(
                pathEditInfoView.PointEditorPanelPrefab,
                pathEditInfoView.PointsParent));
            _pathEditorController = pathEditorController;
            _name = name;
            
            _pathEditInfoView.NameFieldText.onValueChanged.RemoveAllListeners();
            _pathEditInfoView.NameFieldText.onValueChanged
                .AddListener(ChangePathDataName);
            _pathEditInfoView.NameFieldText.text = _name;
            
            _pathEditInfoView.SelectPathButton.onClick.RemoveAllListeners();
            _pathEditInfoView.SelectPathButton.onClick.AddListener(SetCurrentEditing);

            AddDisposable(_pathEditorController.SubscribeOnCurrentPathChange(UpdateSelectSignState));
            AddDisposable(_pathEditorController.SubscribeOnPointsChange(name, UpdatePointsList));

            UpdatePointsList(_pathEditorController.GetPathEditorData(name));
        }

        private void UpdateSelectSignState(string editingPathName)
        {
            bool isActive = editingPathName.Equals(_name);
            _pathEditInfoView.SelectSignObject.SetActive(isActive);
        }

        private void SetCurrentEditing()
        {
            _pathEditorController.SetEditingName(_name);
        }

        private void ChangePathDataName(string newPathName)
        {
            _pathEditorController.ChangeName(_name, newPathName);
            _name = newPathName;
        }

        private void UpdatePointsList(IEnumerable<Hex2d> points)
        {
            _pointEditorPanelViews.ClearList();
            foreach (var point in points)
            {
                AddPoint(point);
            }
        }
        
        public void AddPoint(Hex2d point)
        {
            var element = _pointEditorPanelViews.GetElement();
            element.PointPositionInfo.text = point.ToString();
            
            element.SelectPointButton.onClick.RemoveAllListeners();
            element.SelectPointButton.onClick.AddListener(() => _pathEditorController.SetCurrentInsertNode(_name, point));
            ((RectTransform)_pathEditInfoView.transform).sizeDelta += Vector2.up;
        }
    }
}