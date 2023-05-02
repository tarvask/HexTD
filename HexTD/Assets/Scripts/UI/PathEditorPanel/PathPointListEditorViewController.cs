using HexSystem;
using MapEditor;
using PathSystem;
using UI.Tools;
using UnityEngine;

namespace UI.PathEditorPanel
{
    public class PathPointListEditorViewController : BaseDisposable
    {
        private readonly PathEditInfoView _pathEditInfoView;
        private readonly PathEditorController _pathEditorController;
        private readonly UiElementListPool<PointEditorPanelView> _pointEditorPanelViews;
        private readonly IPathEnumerator _pathEnumerator;

        private byte _pathId;

        public PathEditInfoView PathEditInfoView => _pathEditInfoView;
        public byte PathId => _pathId;

        public PathPointListEditorViewController(PathEditInfoView pathEditInfoView,
            PathEditorController pathEditorController,
            byte pathId)
        {
            _pathEditInfoView = pathEditInfoView;
            _pointEditorPanelViews = AddDisposable(new UiElementListPool<PointEditorPanelView>(
                pathEditInfoView.PointEditorPanelPrefab,
                pathEditInfoView.PointsParent));
            _pathEditorController = pathEditorController;
            _pathId = pathId;

            _pathEnumerator = pathEditorController.GetPathEditorData(_pathId).GetPathEnumerator();
            
            _pathEditInfoView.NameFieldText.onEndEdit.RemoveAllListeners();
            _pathEditInfoView.NameFieldText.onEndEdit.AddListener(ChangePathDataName);
            _pathEditInfoView.NameFieldText.text = _pathId.ToString();
            
            _pathEditInfoView.SelectPathButton.onClick.RemoveAllListeners();
            _pathEditInfoView.SelectPathButton.onClick.AddListener(SetCurrentEditing);

            AddDisposable(_pathEditorController.SubscribeOnCurrentPathChange(UpdateSelectSignState));
            AddDisposable(_pathEditorController.SubscribeOnPointsChange(_pathId, UpdatePointsList));

            UpdatePointsList();
        }

        private void UpdateSelectSignState(byte editingPathId)
        {
            bool isActive = editingPathId.Equals(_pathId);
            _pathEditInfoView.SelectSignObject.SetActive(isActive);
        }

        private void SetCurrentEditing()
        {
            _pathEditorController.SetEditingName(_pathId);
        }

        private void ChangePathDataName(string newPathIdString)
        {
            if (!byte.TryParse(newPathIdString, out byte newPathId))
            {
                Debug.LogWarning("Input only id of path or be sure that id is in byte diapason!");
                _pathEditInfoView.NameFieldText.text = _pathId.ToString();
                return;
            }

            if (_pathEditorController.TryGetPathData(newPathId, out var pathEditorData))
            {
                Debug.LogWarning("Inputted id already exist!");               
                _pathEditInfoView.NameFieldText.text = _pathId.ToString();
                return;
            }
            
            _pathEditorController.ChangeName(_pathId, newPathId);
            _pathId = newPathId;
        }

        private void UpdatePointsList()
        {
            _pointEditorPanelViews.ClearList();
            _pathEnumerator.Reset();
            if(_pathEnumerator.IsEmpty)
                return;
            
            AddPoint(_pathEnumerator.Current);
            while (_pathEnumerator.MoveNext())
            {
                AddPoint(_pathEnumerator.Current);   
            }
        }

        public void AddPoint(Hex2d point)
        {
            var element = _pointEditorPanelViews.GetElement();
            element.PointPositionInfo.text = point.ToString();
            
            element.SelectPointButton.onClick.RemoveAllListeners();
            element.SelectPointButton.onClick.AddListener(() => _pathEditorController.SetCurrentInsertNode(_pathId, point));
            ((RectTransform)_pathEditInfoView.transform).sizeDelta += Vector2.up;
        }
    }
}