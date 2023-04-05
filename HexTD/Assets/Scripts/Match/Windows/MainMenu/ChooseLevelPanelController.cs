using System.Collections.Generic;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Windows.MainMenu
{
    public class ChooseLevelPanelController : BaseDisposable
    {
        public struct Context
        {
            public MatchesConfig LevelsConfig { get; }
            public ChooseLevelPanelView View { get; }
            public ReactiveCommand<int> LevelIndexSelectedReactiveCommand { get; }

            public Context(MatchesConfig levelsConfig, ChooseLevelPanelView view,
                ReactiveCommand<int> levelIndexSelectedReactiveCommand)
            {
                LevelsConfig = levelsConfig;
                View = view;
                LevelIndexSelectedReactiveCommand = levelIndexSelectedReactiveCommand;
            }
        }
        
        private readonly Context _context;
        private List<ChooseLevelItemController> _levelsItems;

        public ChooseLevelPanelController(Context context)
        {
            _context = context;
        }
        
        public void Show()
        {
            if (_levelsItems == null)
                FillItems();
            
            _context.View.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _context.View.gameObject.SetActive(false);
        }

        private void FillItems()
        {
            _levelsItems = new List<ChooseLevelItemController>(_context.LevelsConfig.Levels.Length);
            
            for (int levelIndex = 0; levelIndex < _context.LevelsConfig.Levels.Length; levelIndex++)
            {
                ChooseLevelItemView chooseLevelItemView = Object.Instantiate(_context.View.LevelItemPrefab, _context.View.LevelItemsRoot);
                ChooseLevelItemController.Context chooseLevelItemContext = new ChooseLevelItemController.Context(
                    chooseLevelItemView, _context.LevelsConfig.Levels[levelIndex].name, levelIndex, _context.LevelIndexSelectedReactiveCommand);
                ChooseLevelItemController chooseLevelItem = AddDisposable(new ChooseLevelItemController(chooseLevelItemContext));
                
                _levelsItems.Add(chooseLevelItem);
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _levelsItems.Clear();
        }
    }
}