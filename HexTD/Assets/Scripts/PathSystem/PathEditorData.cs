﻿using System;
using System.Collections.Generic;
using HexSystem;
using Newtonsoft.Json;
using UniRx;

namespace PathSystem
{
    public class PathEditorData : PathData, IDisposable
    {
        [JsonIgnore] private readonly ReactiveCommand _onPointsChangeReactiveCommand;
        [JsonIgnore] private LinkedListNode<Hex2d> _currentInsertNode;

        public PathEditorData([JsonProperty("Name")] byte pathId,
            [JsonProperty("Points")] LinkedList<Hex2d> points) : base(pathId, points)
        {
            _onPointsChangeReactiveCommand = new ReactiveCommand();
        }

        public PathEditorData(byte pathId) : base(pathId, new List<Hex2d>())
        {
            _onPointsChangeReactiveCommand = new ReactiveCommand();
            Reset();
        }

        public PathEditorData(SavePathData pathData) : base(pathData.PathId, pathData.Points)
        {
            _onPointsChangeReactiveCommand = new ReactiveCommand();
            Reset();
        }

        public void SetCurrentInsertNode(Hex2d point)
        {
            for(var node = Points.First; node == null; node = node.Next)
            {
                if (node.Value.Equals(point))
                {
                    _currentInsertNode = node;
                    break;
                }
            }
        }

        private void AddPoint(Hex2d newPoint)
        {
            if (_currentInsertNode == null)
            {
                _currentInsertNode = Points.AddFirst(newPoint);
            }
            else
            {
                _currentInsertNode = Points.AddAfter(_currentInsertNode, newPoint);
            }
            
            _onPointsChangeReactiveCommand.Execute();
        }

        private void RemovePoint(LinkedListNode<Hex2d> removingNode)
        {
            var prevNode = removingNode.Previous;
            if (prevNode == null)
                prevNode = removingNode.Next;

            Points.Remove(removingNode);
            _currentInsertNode = prevNode;
            _onPointsChangeReactiveCommand.Execute();
        }

        private void Reset()
        {
            _currentInsertNode = Points.Last;
        }

        public void HandleAddCommand(Hex2d inputHex)
        {
            var inputHexNode = Points.Find(inputHex);
            bool isHexInPath = inputHexNode != null;
         
            if (!isHexInPath)
                AddPoint(inputHex);
            else
                _currentInsertNode = inputHexNode;
        }

        public void HandleRemoveCommand(Hex2d inputHex)
        {
            var inputHexNode = Points.Find(inputHex);
            bool isHexInPath = inputHexNode != null;
            
            if(isHexInPath) 
                RemovePoint(inputHexNode);
            else
                Reset();
        }

        public void SetPathId(byte pathId)
        {
            PathId = pathId;
        }

        public IDisposable SubscribeOnPointsChange(Action onPointsChange)
        {
            return _onPointsChangeReactiveCommand.Subscribe(_ => onPointsChange.Invoke());
        }

        public void Dispose()
        {
            _onPointsChangeReactiveCommand.Dispose();
        }
    }
}