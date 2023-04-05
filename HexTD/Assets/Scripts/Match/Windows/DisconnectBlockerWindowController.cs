using UniRx;

namespace Match.Windows
{
    public class DisconnectBlockerWindowController : BaseWindowController
    {
        public struct Context
        {
            public DisconnectBlockerWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }

            public Context(DisconnectBlockerWindowView view,
                IReactiveProperty<int> openWindowsCountReactiveProperty)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
            }
        }
        
        public DisconnectBlockerWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
        }

        public void Hide()
        {
            HideWindow();
        }
    }
}