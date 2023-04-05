using Tools;
using UniRx;

namespace Match.Windows.MainMenu
{
    public class ChooseLevelItemController : BaseDisposable
    {
        public struct Context
        {
            public ChooseLevelItemView View { get; }
            public string LevelName { get; }
            public int LevelIndex { get; }
            public ReactiveCommand<int> SelectLevelReactiveCommand { get; }

            public Context(ChooseLevelItemView view, string levelName, int levelIndex,
                ReactiveCommand<int> selectLevelReactiveCommand)
            {
                View = view;
                LevelName = levelName;
                LevelIndex = levelIndex;
                SelectLevelReactiveCommand = selectLevelReactiveCommand;
            }
        }

        private readonly Context _context;

        public ChooseLevelItemController(Context context)
        {
            _context = context;

            _context.View.LevelNameText.text = _context.LevelName;
            _context.View.SelectLevelButton.onClick.AddListener(() =>
                _context.SelectLevelReactiveCommand.Execute(_context.LevelIndex));
        }
    }
}