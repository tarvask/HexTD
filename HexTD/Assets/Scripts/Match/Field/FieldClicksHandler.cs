using System.Collections.Generic;
using HexSystem;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldClicksHandler : BaseDisposable
    {
        public struct Context
        {
            public Vector2 BottomLeftCornerPosition { get; }
            public float TargetCellSizeInUnits { get; }
            
            public ReactiveCommand<Hex2d> InputEvent { get; }

            public Context(Vector2 bottomLeftCornerPosition,
                            float targetCellSizeInUnits,
                            ReactiveCommand<Hex2d> inputEvent)
            {
                BottomLeftCornerPosition = bottomLeftCornerPosition;
                TargetCellSizeInUnits = targetCellSizeInUnits;

                InputEvent = inputEvent;
            }
        }
        
        private const byte QueueSize = 1;
        private readonly Context _context;
        // can be changed to stack to handle last click instead of first
        private readonly Queue<Hex2d> _clickedCells;
        
        public int ClickedCellsCount => _clickedCells.Count;

        public FieldClicksHandler(Context context)
        {
            _context = context;
            
            _clickedCells = new Queue<Hex2d>(QueueSize);
            _context.InputEvent.Subscribe(OnFieldClick);
        }

        private void OnFieldClick(Hex2d clickedHex)
        {
            _clickedCells.Enqueue(clickedHex);
        }

        public Hex2d GetClickedCell()
        {
            Hex2d firstClickedCell = _clickedCells.Dequeue();
            _clickedCells.Clear();
            return firstClickedCell;
        }
    }
}