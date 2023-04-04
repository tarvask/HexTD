using System;
using System.Collections.Generic;
using HexSystem;
using MapEditor;
using UI.Tools;

namespace UI.PathEditorPanel
{
    public class PathPointListEditorViewController : BaseDisposable
    {
        private readonly PathEditInfoView _pathEditInfoView;
        private readonly PathEditorController _pathEditorController;
        private readonly UiElementListPool<PointEditorPanelView> _pointEditorPanelViews;

        private readonly string _name;
        private readonly Action<string> _onPathRemove;

        public PathEditInfoView PathEditInfoView => _pathEditInfoView;

        public PathPointListEditorViewController(PathEditInfoView pathEditInfoView,
            PathEditorController pathEditorController,
            string name,
            Action<string> onPathRemove)
        {
            _pathEditInfoView = pathEditInfoView;
            _pointEditorPanelViews = AddDisposable(new UiElementListPool<PointEditorPanelView>(
                pathEditInfoView.PointEditorPanelPrefab,
                pathEditInfoView.PointsParent));
            _pathEditorController = pathEditorController;
            _onPathRemove = onPathRemove;
            _name = name;

            _pathEditInfoView.DeletePathButton.onClick.RemoveAllListeners();
            _pathEditInfoView.DeletePathButton.onClick.AddListener(OnDeletePath);
            
            _pathEditInfoView.NameFieldText.onValueChanged.RemoveAllListeners();
            _pathEditInfoView.NameFieldText.onValueChanged
                .AddListener(_pathEditorController.SetEditingName);
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
            element.SelectPointButton.onClick.AddListener(() => _pathEditorController.SetCurrentInsertNode(point));
        }

        private void OnDeletePath()
        {
            _onPathRemove.Invoke(_name);
        }
    }
}