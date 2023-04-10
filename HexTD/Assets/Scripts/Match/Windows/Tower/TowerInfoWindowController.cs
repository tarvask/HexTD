using System;
using Match.Field.Tower;
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
        
        public TowerInfoWindowController(Context context)
            : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;
            
            _context.View.CloseButton.onClick.AddListener(CloseWindow);
        }
        
        public void ShowWindow(TowerParameters towerParameters, int towerLevel, //Dictionary<int, AbstractBuffModel> buffs,
            Action onCloseWindowClickedHandler)
        {
            _onCloseWindowClickAction = onCloseWindowClickedHandler;

            _context.View.TowerNameText.text = towerParameters.RegularParameters.Data.TowerName;
            _context.View.TowerDamageText.text = $"{towerParameters.Levels[towerLevel - 1].LevelRegularParams.Data.AttackPower}";
            _context.View.TowerAttackRateText.text =
                $"{(1f / towerParameters.Levels[towerLevel - 1].LevelRegularParams.Data.ReloadTime):F2} / sec";
            _context.View.TowerRangeText.text = $"{towerParameters.Levels[towerLevel - 1].LevelRegularParams.Data.AttackRadiusInHexCount}";
            _context.View.TowerTargetText.text = $"{towerParameters.RegularParameters.Data.TargetFindingTacticType}";
            
            // buffs
            // if (buffs != null)
            // {
            //     foreach (KeyValuePair<int, AbstractBuffModel> buffPair in buffs)
            //     {
            //         TextMeshProUGUI buffInfoElement = Object.Instantiate(_context.View.BuffInfoElementPrefab, _context.View.BuffsInfoScrollRoot);
            //         buffInfoElement.text = buffPair.Value.GetTextInfo();
            //     }
            // }

            base.ShowWindow();
        }

        public void CloseWindow()
        {
            _onCloseWindowClickAction?.Invoke();
            HideWindow();

            // clear buff info elements
            foreach (Transform buffInfoElementTransform in _context.View.BuffsInfoScrollRoot)
            {
                Object.Destroy(buffInfoElementTransform.gameObject);
            }
        }
    }
}