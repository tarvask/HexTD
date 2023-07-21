using Match.Windows;
using Tools;
using UniRx;
using UnityEngine;

namespace Match
{
    public class MatchRulesController : BaseDisposable
    {
        public struct Context
        {
            public WinLoseWindowController WinLoseWindowController { get; }
            public ReactiveCommand EnemyCastleDestroyedReactiveCommand { get; }
            public ReactiveCommand OurCastleDestroyedReactiveCommand { get; }
            public ReactiveCommand EndMatchReactiveCommand { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }
            
            public Context(WinLoseWindowController winLoseWindowController,
                ReactiveCommand enemyCastleDestroyedReactiveCommand,
                ReactiveCommand ourCastleDestroyedReactiveCommand,
                ReactiveCommand endMatchReactiveCommand,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty)
            {
                WinLoseWindowController = winLoseWindowController;
                
                EnemyCastleDestroyedReactiveCommand = enemyCastleDestroyedReactiveCommand;
                OurCastleDestroyedReactiveCommand = ourCastleDestroyedReactiveCommand;
                EndMatchReactiveCommand = endMatchReactiveCommand;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
            }
        }
        
        private readonly Context _context;
        private readonly ReactiveProperty<bool> _isMatchRunning;
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
            {
                Debug.Log($"Match ended on frame {_context.CurrentEngineFrameReactiveProperty.Value} with {_matchResult} result");
                _context.EndMatchReactiveCommand.Execute();
                _context.WinLoseWindowController.ShowWindow(_matchResult);
            }
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