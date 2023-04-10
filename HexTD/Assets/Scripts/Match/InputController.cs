using HexSystem;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match
{
    public class InputController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public HexInteractService HexInteractService { get; }
            public ReactiveCommand<Hex2d> ClickEvent { get; }

            public Context(HexInteractService hexInteractService,
                ReactiveCommand<Hex2d> clickEvent)
            {
                HexInteractService = hexInteractService;
                ClickEvent = clickEvent;
            }
        }

        private readonly Context _context;
        private bool _isInteractable;

        public InputController(Context context)
        {
            _context = context;
            _isInteractable = true;
        }

        public void OuterLogicUpdate(float frameLength)
        {
            GetInput();
        }
        
        private void LockInput()
        {
            _isInteractable = false;
        }
        
        private void UnlockInput()
        {
            _isInteractable = true;
        }

        private void GetInput()
        {
            if (!_isInteractable)
                return;
            
            if (Input.GetMouseButtonUp(0))
            {
                if(_context.HexInteractService.TryClickHexTile(out Hex2d clickedHex))
                    _context.ClickEvent.Execute(clickedHex);
            }
        }
    }
}