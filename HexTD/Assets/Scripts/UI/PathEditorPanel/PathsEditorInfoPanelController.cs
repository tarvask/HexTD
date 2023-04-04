using System.Collections.Generic;
using MapEditor;
using UI.Tools;

namespace UI.PathEditorPanel
{
    public class PathsEditorInfoPanelController : BaseDisposable
    {
        private const string PathDefaultName = "New Path";
        
        private readonly UiElementListPool<PathEditInfoView> _pathsEditorViewPool;
        private readonly Dictionary<string, PathPointListEditorViewController> _pathPointListEditorViewControllers;
        private readonly PathsEditorInfoPanelView _pathsEditorInfoPanelView;
        private readonly PathEditorController _pathEditorController;

        public PathsEditorInfoPanelController(PathsEditorInfoPanelView pathsEditorInfoPanelView,
            PathEditorController pathEditorController)
        {
            _pathPointListEditorViewControllers = new Dictionary<string, PathPointListEditorViewController>();
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
                AddPathViewElement(path.Name);
            }
        }

        private void OnNewPathAdd()
        {
            string pathEditorDataName = GeneratePathName();
            _pathEditorController.AddPath(pathEditorDataName);
            AddPathViewElement(pathEditorDataName);
        }

        private string GeneratePathName()
        {
            string pathEditorDataName = PathDefaultName;
            for (int i = _pathPointListEditorViewControllers.Count + 1;
                 _pathPointListEditorViewControllers.ContainsKey(pathEditorDataName);
                 i++)
            {
                pathEditorDataName = $"{PathDefaultName} {i}";
            }

            return pathEditorDataName;
        }

        private void AddPathViewElement(string name)
        {
            if(_pathPointListEditorViewControllers.ContainsKey(name))
                return;
            
            var pathPointListEditorView = _pathsEditorViewPool.GetElement();
            var pathPointListEditorViewController = new PathPointListEditorViewController(
                pathPointListEditorView, _pathEditorController, name, OnPathRemove);
            _pathPointListEditorViewControllers.Add(name, pathPointListEditorViewController);
            
            pathPointListEditorView.DeletePathButton.onClick.RemoveAllListeners();
            pathPointListEditorView.DeletePathButton.onClick.AddListener(() => 
                OnPathRemove(name));
        }
        
        private void PathRemove(string pathEditorDataName)
        {   
            if(!_pathPointListEditorViewControllers
                   .Remove(pathEditorDataName, out var pathPointListEditorViewController))
                return;
            
            pathPointListEditorViewController.Dispose();
            _pathsEditorViewPool.RemoveElement(pathPointListEditorViewController.PathEditInfoView);
        }

        private void OnPathRemove(string pathEditorDataName)
        {
            PathRemove(pathEditorDataName);
            _pathEditorController.RemovePath(pathEditorDataName);
        }
    }
}