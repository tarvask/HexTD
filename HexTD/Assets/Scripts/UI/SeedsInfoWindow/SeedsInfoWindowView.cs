using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.SeedsInfoWindow
{
    public class SeedsInfoWindowView : WindowViewBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _temperatureSlider;

        [SerializeField] private RangeSlider _rangeSlider;

        [SerializeField] private int[] _temperatureValues;

        public IObservable<Unit> CloseButtonClick => _closeButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        protected override void DoAwake()
        {
            _rangeSlider.CalculateSliderPosition(_temperatureSlider, _temperatureValues[0], _temperatureValues[1]);
        }
    }
}