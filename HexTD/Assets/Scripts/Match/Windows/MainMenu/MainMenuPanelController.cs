using Tools;
using UniRx;

namespace Match.Windows.MainMenu
{
    public class MainMenuPanelController : BaseDisposable
    {
        public struct Context
        {
            public MainMenuPanelView View { get; }
            public ReactiveCommand GameButtonClickedReactiveCommand { get; }
            public ReactiveCommand TowersButtonClickedReactiveCommand { get; }

            public Context(MainMenuPanelView view,
                ReactiveCommand gameButtonClickedReactiveCommand, ReactiveCommand towersButtonClickedReactiveCommand)
            {
                View = view;
                GameButtonClickedReactiveCommand = gameButtonClickedReactiveCommand;
                TowersButtonClickedReactiveCommand = towersButtonClickedReactiveCommand;
            }
        }

        private readonly Context _context;

        public MainMenuPanelController(Context context)
        {
            _context = context;
            
            _context.View.GameButton.onClick.AddListener(() =>_context.GameButtonClickedReactiveCommand.Execute(Unit.Default));
            _context.View.TowersButton.onClick.AddListener(() =>_context.TowersButtonClickedReactiveCommand.Execute(Unit.Default));
        }
    }
}