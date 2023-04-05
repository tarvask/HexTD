using Tools.Interfaces;
using UniRx;

namespace Match.Windows
{
    public class WaveStartInfoWindowController : BaseWindowController, IOuterLogicUpdatable
    {
        public struct Context
        {
            public WaveStartInfoWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }
            
            public Context(WaveStartInfoWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
            }
        }
        
        private readonly Context _context;
        private float _targetShowDuration;
        private float _currentShowDuration;
        
        public WaveStartInfoWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;
        }
        
        public void ShowWindow(float showDuration)
        {
            _targetShowDuration = showDuration;
            base.ShowWindow();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _currentShowDuration += frameLength;
            
            if (_currentShowDuration >= _targetShowDuration)
                HideWindow();
        }
    }
}