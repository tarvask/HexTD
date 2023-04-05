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
            public Camera MainCamera { get; }
            public InputAreaInWorld InputAreaInWorld { get; }
            public ReactiveCommand<Vector2> ClickEvent { get; }
            public IReadOnlyReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }

            public Context(Camera mainCamera, InputAreaInWorld inputAreaInWorld,
                ReactiveCommand<Vector2> clickEvent,
                IReadOnlyReactiveProperty<int> openWindowsCountReactiveProperty)
            {
                MainCamera = mainCamera;
                InputAreaInWorld = inputAreaInWorld;
                ClickEvent = clickEvent;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
            }
        }

        private readonly Context _context;
        private bool _isInteractable;

        public InputController(Context context)
        {
            _context = context;
            
            _context.OpenWindowsCountReactiveProperty.Subscribe(OpenWindowsCountChangedHandler);
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
        
        private void OpenWindowsCountChangedHandler(int openWindowsCount)
        {
            _isInteractable = openWindowsCount == 0;
        }

        private void GetInput()
        {
            if (!_isInteractable)
                return;
            
            //========== Для инпута через TouchPanel на screen space-overlay канвасе ============
//            if (Input.GetMouseButtonUp(0))
//            {
//                Rect touchPanelScreenRect = TouchPanelRect;
//                Vector3 mousePosition = _context.MainCamera.ViewportToWorldPoint(Input.mousePosition);
//                
////                Vector3 worldPosition = _context.MainCamera.ScreenToWorldPoint(mousePosition);
////                Vector3 fieldPosition = worldPosition - _context.FieldRootPosition;
//                
////                Debug.Log($"Touched {mousePosition} in pixels, {worldPosition} in units, {fieldPosition} on field");
//
//                float touchPanelClickViewportPositionX = Mathf.InverseLerp(touchPanelScreenRect.xMin,
//                                                                       touchPanelScreenRect.xMax,
//                                                                       mousePosition.x);
//                float touchPanelClickViewportPositionY = Mathf.InverseLerp(touchPanelScreenRect.yMin,
//                                                                       touchPanelScreenRect.yMax,
//                                                                       mousePosition.y);
//
//                //_context.ClickEvent.Fire(new Vector2(fieldPosition.x, fieldPosition.y));
//                _context.ClickEvent.Fire(new Vector2(touchPanelClickViewportPositionX, touchPanelClickViewportPositionY));
//            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 inputAreaPosition = _context.InputAreaInWorld.transform.position;
                Vector2 inputAreaOnScreenMin = _context.MainCamera.WorldToScreenPoint(
                    inputAreaPosition + _context.InputAreaInWorld.AreaRect.min);
                Vector2 inputAreaOnScreenMax = _context.MainCamera.WorldToScreenPoint(
                    inputAreaPosition + _context.InputAreaInWorld.AreaRect.max);

                float inputAreaClickPosX = Mathf.InverseLerp(inputAreaOnScreenMin.x,
                                                             inputAreaOnScreenMax.x,
                                                             Input.mousePosition.x);
                float inputAreaClickPosY = Mathf.InverseLerp(inputAreaOnScreenMin.y,
                                                             inputAreaOnScreenMax.y,
                                                             Input.mousePosition.y);

                _context.ClickEvent.Execute(new Vector2(inputAreaClickPosX, inputAreaClickPosY));
            }
        }
    }
}