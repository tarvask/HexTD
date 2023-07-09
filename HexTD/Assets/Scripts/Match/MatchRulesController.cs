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
        private ReactiveProperty<bool> _isMatchRunning;
        private bool _isEnemyCastleDestroyed, _isOurCastleDestroyed;
        private MatchResultType _matchResult;

        public IReadOnlyReactiveProperty<bool> IsMatchRunning => _isMatchRunning;

        public MatchRulesController(Context context)
        {
            _context = context;
            
            _context.EnemyCastleDestroyedReactiveCommand.Subscribe(OnEnemyCastleDestroyedEventHandler);
            _context.OurCastleDestroyedReactiveCommand.Subscribe(OnOurCastleDestroyedEventHandler);

            _isMatchRunning = AddDisposable(new ReactiveProperty<bool>(true));
            _matchResult = MatchResultType.Undefined;
        }

        public void OuterLogicUpdate(float frameLength)
        {
            if (_isEnemyCastleDestroyed || _isOurCastleDestroyed)
            {
                if (!_isOurCastleDestroyed)
                    _matchResult = MatchResultType.Win;
                else if (!_isEnemyCastleDestroyed)
                    _matchResult = MatchResultType.Lose;
                else
                    _matchResult = MatchResultType.Draw;
            }

            _isMatchRunning.Value = _matchResult == MatchResultType.Undefined;
            
            if (!_isMatchRunning.Value)
                _context.WinLoseWindowController.ShowWindow(_matchResult);
        }

        private void OnEnemyCastleDestroyedEventHandler(Unit unit)
        {
            _isEnemyCastleDestroyed = true;
        }

        private void OnOurCastleDestroyedEventHandler(Unit unit)
        {
            _isOurCastleDestroyed = true;
        }
    }
}