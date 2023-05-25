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
        private readonly ReactiveCommand<byte> _onPathAdd;
        private readonly ReactiveCommand<byte> _onPathRemove;
        private readonly ReactiveProperty<byte> _editingPathId;
        private readonly Layout _layout;
        
        private readonly List<byte> _pathIds;

        public PathEditorController(Layout layout, EditorPathContainer editorPathContainer)
        {
            _pathsContainer = editorPathContainer;
            _onPathAdd = new ReactiveCommand<byte>();
            _onPathRemove = new ReactiveCommand<byte>();
            _editingPathId = new ReactiveProperty<byte>(byte.MaxValue);
            _layout = layout;

            _pathIds = new List<byte>(10);
        }

        public void DrawPath()
        {
            if(!_pathsContainer.TryGetPathData(_editingPathId.Value, out var pathData))
                return;

            using var enumerator = pathData.GetEnumerator();
            Hex2d pointStart = enumerator.Current;
            while(enumerator.MoveNext())
            {
                Debug.DrawLine(_layout.ToPlane(pointStart), _layout.ToPlane(enumerator.Current), Color.black);
                pointStart = enumerator.Current;
            }
        }

        public void SetEditingName(byte newEditingPathId)
        {
            _editingPathId.Value = newEditingPathId;
        }

        public void ChangeName(byte oldPathId, byte newEditingPathId)
        {
            _pathsContainer.ChangeName(oldPathId, newEditingPathId);
            _editingPathId.Value = newEditingPathId;
        }

        public void SetCurrentInsertNode(Hex2d point)
        {
            _pathsContainer[_editingPathId.Value].SetCurrentInsertNode(point);
        }

        public void SetCurrentInsertNode(byte pathId, Hex2d point)
        {
            SetEditingName(pathId);
            SetCurrentInsertNode(point);
        }

        public void AddPath(byte pathId)
        {
            if(!_pathsContainer.TryAddPath(pathId))
                return;

            SetEditingName(pathId);
            _onPathAdd.Execute(pathId);
        }

        public void AddPath(PathData.SavePathData savePathData)
        {
            _pathsContainer.AddPath(savePathData);
            _onPathAdd.Execute(savePathData.PathId);
            SetEditingName(savePathData.PathId);
        }

        public void RemovePath(byte pathId)
        {
            if(!_pathsContainer.TryRemove(pathId))
                return;

            _onPathRemove.Execute(pathId);
            if (_editingPathId.Value.Equals(pathId))
                _editingPathId.Value = _pathsContainer.GetLastName();
        }
        
        public void LmbClickHandle(Hex2d hex)
        {
            if(_editingPathId.Value.Equals(byte.MaxValue))
                return;
            
            _pathsContainer[_editingPathId.Value]?.HandleAddCommand(hex);
        }

        public void RmbClickHandle(Hex2d hex)
        {
            if(_editingPathId.Value.Equals(byte.MaxValue))
                return;
            
            _pathsContainer[_editingPathId.Value]?.HandleRemoveCommand(hex);
        }
        
        public IDisposable SubscribeOnPointsChange(byte pathId, Action onPointsChange)
        {
            return _pathsContainer[pathId].SubscribeOnPointsChange(onPointsChange);
        }
        
        public IDisposable SubscribeOnPathAdd(Action<byte> onPathAdd)
        {
            return _onPathAdd.Subscribe(onPathAdd);
        }
        
        public IDisposable SubscribeOnPathRemove(Action<byte> onPathRemove)
        {
            return _onPathRemove.Subscribe(onPathRemove);
        }
        
        public IDisposable SubscribeOnCurrentPathChange(Action<byte> onCurrentPathChange)
        {
            return _editingPathId.Subscribe(onCurrentPathChange);
        }

        public PathEditorData GetPathEditorData(byte pathId)
        {
            return _pathsContainer[pathId];
        }

        public bool TryGetPathData(byte pathId, out PathEditorData pathEditorData)
        {
            return _pathsContainer.TryGetPathData(pathId, out pathEditorData);
        }
        

        public void Clear()
        {
            _pathsContainer.GetNames(_pathIds);
            foreach (var pathId in _pathIds)
            {
                RemovePath(pathId);
            }
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