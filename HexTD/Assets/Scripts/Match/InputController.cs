using HexSystem;
using Lean.Touch;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match
{
    public class InputController : BaseDisposable
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

        public InputController(Context context)
        {
            _context = context;
            LeanTouch.OnFingerDown += GetInput;
        }

        private void GetInput(LeanFinger finger)
        {
            if (finger.IsOverGui)
                return;

            if (_context.HexInteractService.TryClickHexTile(finger, out Hex2d clickedHex))
                _context.ClickEvent.Execute(clickedHex);
        }
    }
}