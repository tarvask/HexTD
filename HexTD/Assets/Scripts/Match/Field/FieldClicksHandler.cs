using System.Collections.Generic;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldClicksHandler : BaseDisposable
    {
        public struct Context
        {
            public int FieldWidth { get; }
            public int FieldHeight { get; }
            public Vector2 BottomLeftCornerPosition { get; }
            public float TargetCellSizeInUnits { get; }
            
            public ReactiveCommand<Vector2> InputEvent { get; }

            public Context(int fieldWidth, int fieldHeight,
                            Vector2 bottomLeftCornerPosition,
                            float targetCellSizeInUnits,
                            ReactiveCommand<Vector2> inputEvent)
            {
                FieldWidth = fieldWidth;
                FieldHeight = fieldHeight;
                BottomLeftCornerPosition = bottomLeftCornerPosition;
                TargetCellSizeInUnits = targetCellSizeInUnits;

                InputEvent = inputEvent;
            }
        }
        
        private const byte QueueSize = 1;
        private readonly Context _context;
        // can be changed to stack to handle last click instead of first
        private readonly Queue<Vector2Int> _clickedCells;
        
        public int ClickedCellsCount => _clickedCells.Count;

        public FieldClicksHandler(Context context)
        {
            _context = context;
            
            _clickedCells = new Queue<Vector2Int>(QueueSize);
            _context.InputEvent.Subscribe(OnFieldClick);
        }
        
        private void OnFieldClick(Vector2 clickPosition)
        {
            if (IsClickOnField(clickPosition, out Vector2Int clickedCell))
            {
                _clickedCells.Enqueue(clickedCell);
            }
        }

        public Vector2Int GetClickedCell()
        {
            Vector2Int firstClickedCell = _clickedCells.Dequeue();
            _clickedCells.Clear();
            return firstClickedCell;
        }

        private bool IsClickOnFieldOld(Vector2 clickPosition, out Vector2Int clickedCell)
        {
            clickedCell = -Vector2Int.one;
            Vector2 clickRelativePosition = clickPosition - _context.BottomLeftCornerPosition;

            float xCellNumberFloat = clickRelativePosition.x / _context.TargetCellSizeInUnits;
            float yCellNumberFloat = clickRelativePosition.y / _context.TargetCellSizeInUnits;

            if (xCellNumberFloat > 0 && yCellNumberFloat > 0)
            {
                int xCellNumber = Mathf.FloorToInt(xCellNumberFloat);
                int yCellNumber = Mathf.FloorToInt(yCellNumberFloat);

                if (xCellNumber < _context.FieldWidth && yCellNumber < _context.FieldHeight)
                {
                    clickedCell = new Vector2Int(xCellNumber, yCellNumber);
                    return true;
                }
            }
            
            return false;
        }
        
        private bool IsClickOnField(Vector2 clickPosition, out Vector2Int clickedCell)
        {
            clickedCell = -Vector2Int.one;
            
            if (1 - clickPosition.x < 0.001f || clickPosition.x < 0.001f
                                             || 1 - clickPosition.y < 0.001f
                                             || clickPosition.y < 0.001f)
                return false;
            
            Vector2 fieldCellTemp = new Vector2(clickPosition.x * _context.FieldWidth,
                                                clickPosition.y * _context.FieldHeight);
            clickedCell = FieldCellsTools.GetCellForFieldPosition(fieldCellTemp);
            
            return true;
        }
        
        public bool IsClickValuable(Vector2Int clickedCell)
        {
            return true;
            //return componentModel.Items[clickedCell.y, clickedCell.x].IsValuableForClick();
        }
    }
}