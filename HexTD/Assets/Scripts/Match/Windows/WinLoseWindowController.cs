using UniRx;

namespace Match.Windows
{
    public class WinLoseWindowController : BaseWindowController
    {
        public struct Context
        {
            public WinLoseWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }
            public ReactiveCommand QuitMatchEndedReactiveCommand { get; }
            
            public Context(WinLoseWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty,
                ReactiveCommand quitMatchEndedReactiveCommand)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
                QuitMatchEndedReactiveCommand = quitMatchEndedReactiveCommand;
            }
        }
        
        private readonly Context _context;
        
        public WinLoseWindowController(Context context) : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;
            
            _context.View.CloseButton.onClick.AddListener(HideWindow);
            _context.View.CloseButton.onClick.AddListener(() => _context.QuitMatchEndedReactiveCommand.Execute());
        }
        
        public void ShowWindow(bool isOurWin)
        {
            _context.View.WinGroup.SetActive(isOurWin);
            _context.View.LoseGroup.SetActive(!isOurWin);
            
            base.ShowWindow();
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            
            _context.View.CloseButton.onClick.RemoveAllListeners();
        }
    }
}