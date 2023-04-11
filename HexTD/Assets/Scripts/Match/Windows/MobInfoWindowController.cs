using System;
using Match.Field.Mob;
using UniRx;
using UnityEngine;

namespace Match.Windows
{
    public class MobInfoWindowController : BaseWindowController
    {
        public struct Context
        {
            public MobInfoWindowView View { get; }
            public IReactiveProperty<int> OpenWindowsCountReactiveProperty { get; }

            public Context(MobInfoWindowView view, IReactiveProperty<int> openWindowsCountReactiveProperty)
            {
                View = view;
                OpenWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
            }
        }

        private readonly Context _context;
        private Action _onCloseWindowClickAction;
        
        public MobInfoWindowController(Context context)
            : base(context.View, context.OpenWindowsCountReactiveProperty)
        {
            _context = context;
            
            _context.View.CloseButton.onClick.AddListener(OnCloseWindowClickedHandler);
        }
        
        public void ShowWindow(MobParameters mobParameters, Sprite mobIcon, Action onCloseWindowClickedHandler)
        {
            _onCloseWindowClickAction = onCloseWindowClickedHandler;

            _context.View.MobNameText.text = $"";
            _context.View.MobPowerText.text = $"{mobParameters.PowerType}";
            _context.View.MobIcon.sprite = mobIcon;
            
            _context.View.MobPriceText.text = $"{mobParameters.PriceInSilver}";
            _context.View.MobHealthText.text = $"{mobParameters.HealthPoints}";;
            _context.View.MobAttackPowerText.text = $"{mobParameters.AttackPower}";
            _context.View.MobAttackRateText.text = $"{(1f / mobParameters.ReloadTime):F2} / sec";
            _context.View.MobSpeedText.text = $"{mobParameters.Speed}";
            _context.View.MobRewardText.text = $"{mobParameters.RewardInSilver}";

            base.ShowWindow();
        }
        
        private void OnCloseWindowClickedHandler()
        {
            _onCloseWindowClickAction.Invoke();
            HideWindow();
        }
    }
}