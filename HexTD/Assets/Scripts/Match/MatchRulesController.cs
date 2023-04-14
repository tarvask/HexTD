using Match.Windows;
using Tools;
using UniRx;

namespace Match
{
    public class MatchRulesController : BaseDisposable
    {
        public struct Context
        {
            public WinLoseWindowController WinLoseWindowController { get; }
            public ReactiveCommand EnemyCastleDestroyedReactiveCommand { get; }
            public ReactiveCommand OurCastleDestroyedReactiveCommand { get; }
            
            public Context(WinLoseWindowController winLoseWindowController,
                ReactiveCommand enemyCastleDestroyedReactiveCommand,
                ReactiveCommand ourCastleDestroyedReactiveCommand)
            {
                WinLoseWindowController = winLoseWindowController;
                
                EnemyCastleDestroyedReactiveCommand = enemyCastleDestroyedReactiveCommand;
                OurCastleDestroyedReactiveCommand = ourCastleDestroyedReactiveCommand;
            }
        }
        
        private readonly Context _context;
        private bool _isMatchRunning;

        public bool IsMatchRunning => _isMatchRunning;

        public MatchRulesController(Context context)
        {
            _context = context;
            
            _context.EnemyCastleDestroyedReactiveCommand.Subscribe(OnEnemyCastleDestroyedEventHandler);
            _context.OurCastleDestroyedReactiveCommand.Subscribe(OnOurCastleDestroyedEventHandler);

            _isMatchRunning = true;
        }

        private void OnEnemyCastleDestroyedEventHandler(Unit unit)
        {
            _isMatchRunning = false;
            _context.WinLoseWindowController.ShowWindow(true);
        }

        private void OnOurCastleDestroyedEventHandler(Unit unit)
        {
            _isMatchRunning = false;
            _context.WinLoseWindowController.ShowWindow(false);
        }
    }
}