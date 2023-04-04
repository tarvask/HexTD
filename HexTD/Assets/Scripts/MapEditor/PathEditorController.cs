using System;
using System.Collections;
using System.Collections.Generic;
using HexSystem;
using InputSystem;
using PathSystem;
using UniRx;
using UnityEngine;

namespace MapEditor
{
    public class PathEditorController : IPointerInputListener, IEnumerable<PathEditorData>
    {
        private readonly EditorPathContainer _pathsContainer;
        private readonly ReactiveCommand<string> _onPathAdd;
        private readonly ReactiveCommand<string> _onPathRemove;
        private readonly ReactiveProperty<string> _editingPathName;
        private readonly Layout _layout;

        public PathEditorController(Layout layout, EditorPathContainer editorPathContainer)
        {
            _pathsContainer = editorPathContainer;
            _onPathAdd = new ReactiveCommand<string>();
            _onPathRemove = new ReactiveCommand<string>();
            _editingPathName = new ReactiveProperty<string>("");
            _layout = layout;
        }

        public void DrawPath()
        {
            if(!_pathsContainer.TryGetPathData(_editingPathName.Value, out var pathData))
                return;

            using var enumerator = pathData.GetEnumerator();
            Hex2d pointStart = enumerator.Current;
            while(enumerator.MoveNext())
            {
                Debug.DrawLine(_layout.ToPlane(pointStart), _layout.ToPlane(enumerator.Current));
                pointStart = enumerator.Current;
            }
        }

        public void SetEditingName(string editingPathName)
        {
            _editingPathName.Value = editingPathName;
        }

        public void SetCurrentInsertNode(Hex2d point)
        {
            _pathsContainer[_editingPathName.Value].SetCurrentInsertNode(point);
        }

        public void AddPath(string name)
        {
            if(!_pathsContainer.TryAddPath(name))
                return;

            SetEditingName(name);
            _onPathAdd.Execute(name);
        }

        public void AddPath(PathData.SavePathData savePathData)
        {
            _pathsContainer.AddPath(savePathData);
            _onPathAdd.Execute(savePathData.Name);
            SetEditingName(savePathData.Name);
        }

        public void RemovePath(string name)
        {
            if(!_pathsContainer.TryRemove(name))
                return;

            _onPathRemove.Execute(name);
            if (_editingPathName.Value.Equals(name))
                _editingPathName.Value = _pathsContainer.GetLastName();
        }
        
        public void LmbClickHandle(Hex2d hex)
        {
            if(_editingPathName.Value.Equals(String.Empty))
                return;
            
            _pathsContainer[_editingPathName.Value]?.HandleAddCommand(hex);
        }

        public void RmbClickHandle(Hex2d hex)
        {
            if(_editingPathName.Value.Equals(String.Empty))
                return;
            
            _pathsContainer[_editingPathName.Value]?.HandleRemoveCommand(hex);
        }
        
        public IDisposable SubscribeOnPointsChange(string name, Action<IEnumerable<Hex2d>> onPointsChange)
        {
            return _pathsContainer[name].SubscribeOnPointsChange(onPointsChange);
        }
        
        public IDisposable SubscribeOnPathAdd(Action<string> onPathAdd)
        {
            return _onPathAdd.Subscribe(onPathAdd);
        }
        
        public IDisposable SubscribeOnPathRemove(Action<string> onPathRemove)
        {
            return _onPathRemove.Subscribe(onPathRemove);
        }
        
        public IDisposable SubscribeOnCurrentPathChange(Action<string> onCurrentPathChange)
        {
            return _editingPathName.Subscribe(onCurrentPathChange);
        }

        public PathEditorData GetPathEditorData(string name)
        {
            return _pathsContainer[name];
        }

        public IEnumerator<PathEditorData> GetEnumerator()
        {
            return _pathsContainer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}