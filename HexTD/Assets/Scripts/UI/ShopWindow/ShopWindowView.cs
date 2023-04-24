using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.ShopwWindow
{
    public class ShopWindowView : WindowViewBase
    {
        [SerializeField] private Button _closeButton;

        public IObservable<Unit> CloseButtonClick => _closeButton
            .OnClickAsObservable()
            .WhereAppeared(this);
    }
}