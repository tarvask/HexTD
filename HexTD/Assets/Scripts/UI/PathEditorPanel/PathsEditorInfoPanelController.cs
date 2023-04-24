using System.Collections.Generic;
using MapEditor;
using UI.Tools;

namespace UI.PathEditorPanel
{
    public class PathsEditorInfoPanelController : BaseDisposable
    {
        private readonly UiElementListPool<PathEditInfoView> _pathsEditorViewPool;
        private readonly List<PathPointListEditorViewController> _pathPointListEditorViewControllers;
        private readonly PathsEditorInfoPanelView _pathsEditorInfoPanelView;
        private readonly PathEditorController _pathEditorController;

        public PathsEditorInfoPanelController(PathsEditorInfoPanelView pathsEditorInfoPanelView,
            PathEditorController pathEditorController)
        {
            _pathPointListEditorViewControllers = new List<PathPointListEditorViewController>(10);
            _pathsEditorInfoPanelView = pathsEditorInfoPanelView;
            _pathsEditorViewPool = new UiElementListPool<PathEditInfoView>(
                _pathsEditorInfoPanelView.PathEditInfoPrefab,
                _pathsEditorInfoPanelView.PathEditInfoTransformParent);
            _pathEditorController = pathEditorController;

            _pathsEditorInfoPanelView.AddPathButton.onClick.RemoveAllListeners();
            _pathsEditorInfoPanelView.AddPathButton.onClick.AddListener(OnNewPathAdd);
            AddDisposable(_pathEditorController.SubscribeOnPathAdd(AddPathViewElement));
            AddDisposable(_pathEditorController.SubscribeOnPathRemove(PathRemove));

            foreach (var path in _pathEditorController)
            {
                AddPathViewElement(path.PathId);
            }
        }

        private void OnNewPathAdd()
        {
            byte pathEditorId = GenerateAvailablePathId();
            _pathEditorController.AddPath(pathEditorId);
        }

        private byte GenerateAvailablePathId()
        {
            byte i;
            for (i = (byte)(_pathPointListEditorViewControllers.Count + 1);
                 _pathEditorController.TryGetPathData(i, out var _);
                 i++)
            {
            }

            return i;
        }

        private void AddPathViewElement(byte pathId)
        {
            var pathPointListEditorView = _pathsEditorViewPool.GetElement();
            var pathPointListEditorViewController = new PathPointListEditorViewController(
                pathPointListEditorView, _pathEditorController, pathId);
            _pathPointListEditorViewControllers.Add(pathPointListEditorViewController);
            
            pathPointListEditorView.DeletePathButton.onClick.RemoveAllListeners();
            pathPointListEditorView.DeletePathButton.onClick.AddListener(() => 
                PathRemove(pathPointListEditorViewController));
        }
        
        private void PathRemove(PathPointListEditorViewController pathPointListEditorViewController)
        {   
            if(!_pathPointListEditorViewControllers.Remove(pathPointListEditorViewController))
                return;
            
            _pathEditorController.RemovePath(pathPointListEditorViewController.PathId);
            pathPointListEditorViewController.Dispose();
            _pathsEditorViewPool.RemoveElement(pathPointListEditorViewController.PathEditInfoView);
        }

        private void PathRemove(byte pathId)
        {
            var pathPointListEditorViewController = _pathPointListEditorViewControllers.Find(contoller => contoller.PathId == pathId);
            if(pathPointListEditorViewController != null)
                PathRemove(pathPointListEditorViewController);
        }
    }
}