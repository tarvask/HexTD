using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.CropsFilterWindow
{
    public class CropsFilterWindowView : WindowViewBase
    {
        [SerializeField] private Button _confirmButton;

        public IObservable<Unit> ConfirmButtonClick => _confirmButton
            .OnClickAsObservable()
            .WhereAppeared(this);
    }
}