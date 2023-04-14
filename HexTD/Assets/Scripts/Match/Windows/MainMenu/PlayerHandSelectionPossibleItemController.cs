using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Windows.MainMenu
{
    public class PlayerHandSelectionPossibleItemController : BaseDisposable
    {
        public struct Context
        {
            public PlayerHandSelectionPossibleItemView View { get; }
            public TowerConfigNew Config { get; }
            public ReactiveCommand<PlayerHandSelectionPossibleItemController> PossibleTowerItemSelectedReactiveCommand { get; }

            public Context(PlayerHandSelectionPossibleItemView view, TowerConfigNew config,
                ReactiveCommand<PlayerHandSelectionPossibleItemController> possibleTowerItemSelectedReactiveCommand)
            {
                View = view;
                Config = config;
                PossibleTowerItemSelectedReactiveCommand = possibleTowerItemSelectedReactiveCommand;
            }
        }

        private readonly Context _context;

        public TowerConfigNew TowerConfig => _context.Config;
        public RectTransform ItemRectTransform => (RectTransform)_context.View.transform;

        public PlayerHandSelectionPossibleItemController(Context context)
        {
            _context = context;

            _context.View.SetData(_context.Config.RegularParameters);
            
            _context.View.SelectButton.onClick.AddListener(() => _context.PossibleTowerItemSelectedReactiveCommand.Execute(this));
        }
    }
}