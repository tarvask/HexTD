using System;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Windows.Tower
{
    public class TowerInfoWindowController : BaseWindowController
    {
        public struct Context
        {
            public TowerInfoWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }
            
            public Context(TowerInfoWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
            }
        }

        private readonly Context _context;
        private Action _onCloseWindowClickAction;
        private readonly TowerInfoPanelController _infoPanelController;
        
        public TowerInfoWindowController(Context context)
            : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;

            TowerInfoPanelController.Context infoPanelControllerContext =
                new TowerInfoPanelController.Context(_context.View.InfoPanel);
            _infoPanelController = AddDisposable(new TowerInfoPanelController(infoPanelControllerContext));
            
            _context.View.CloseButton.onClick.AddListener(CloseWindow);
        }
        
        public void ShowWindow(TowerConfigNew towerParameters, int towerLevel, //Dictionary<int, AbstractBuffModel> buffs,
            Action onCloseWindowClickedHandler)
        {
            _onCloseWindowClickAction = onCloseWindowClickedHandler;
            
            _infoPanelController.Init(towerParameters, towerLevel);

            base.ShowWindow();
        }

        public void CloseWindow()
        {
            _onCloseWindowClickAction?.Invoke();
            _infoPanelController.Clear();
            HideWindow();
        }
    }
}