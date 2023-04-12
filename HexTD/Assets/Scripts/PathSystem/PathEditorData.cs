using System;
using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Newtonsoft.Json;
using UniRx;

namespace PathSystem
{
    public class PathEditorData : PathData, IDisposable
    {
        [JsonIgnore] private readonly ReactiveCommand<IEnumerable<Hex2d>> _onPointsChangeReactiveCommand;
        [JsonIgnore] private LinkedListNode<Hex2d> _currentInsertNode;

        [JsonIgnore] public List<Hex2d> PointsList => Points.ToList();

        public PathEditorData([JsonProperty("Name")] string name,
            [JsonProperty("Points")] LinkedList<Hex2d> points) : base(name, points)
        {
        }

        public PathEditorData(string name) : base(name, new List<Hex2d>())
        {
            _onPointsChangeReactiveCommand = new ReactiveCommand<IEnumerable<Hex2d>>();
            Reset();
        }

        public PathEditorData(SavePathData pathData) : base(pathData.Name, pathData.Points)
        {
            _onPointsChangeReactiveCommand = new ReactiveCommand<IEnumerable<Hex2d>>();
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
                Points.AddFirst(newPoint);
                _currentInsertNode = Points.Last;
            }
            else
            {
                _currentInsertNode = Points.AddAfter(_currentInsertNode, newPoint);
            }
            
            _onPointsChangeReactiveCommand.Execute(this);
        }

        private void RemovePoint(LinkedListNode<Hex2d> removingNode)
        {
            var prevNode = removingNode.Previous;
            if (prevNode == null)
                prevNode = removingNode.Next;

            Points.Remove(removingNode);
            _currentInsertNode = prevNode;
            _onPointsChangeReactiveCommand.Execute(this);
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

        public void SetName(string name)
        {
            Name = name;
        }

        public IDisposable SubscribeOnPointsChange(Action<IEnumerable<Hex2d>> onPointsChange)
        {
            return _onPointsChangeReactiveCommand.Subscribe(onPointsChange);
        }

        public void Dispose()
        {
            _onPointsChangeReactiveCommand.Dispose();
        }
    }
}