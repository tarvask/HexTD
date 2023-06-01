using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.PlotsFilterWindow
{
    public class PlotsFilterWindowView : WindowViewBase
    {
        [SerializeField] private Button _confirmButton;

        [Header("Temperature")]
        [SerializeField] private RectTransform _temperatureSlider;
        [SerializeField] private TextMeshProUGUI _temperatureMin;
        [SerializeField] private TextMeshProUGUI _temperatureMax;

        [Header("Humidity")]
        [SerializeField] private RectTransform _humididtySlider;
        [SerializeField] private TextMeshProUGUI _humididtyMin;
        [SerializeField] private TextMeshProUGUI _humididtyMax;

        [Header("pH Level")]
        [SerializeField] private RectTransform _phLevelSlider;
        [SerializeField] private TextMeshProUGUI _phLevelMin;
        [SerializeField] private TextMeshProUGUI _phLevelMax;

        [Header("Humus")]
        [SerializeField] private RectTransform _humusSlider;
        [SerializeField] private TextMeshProUGUI _humusMin;
        [SerializeField] private TextMeshProUGUI _humusMax;

        [SerializeField] private RangeSlider _rangeSlider;

        [SerializeField] private int[] _temperatureValuesTest; // for test
        [SerializeField] private int[] _humididtyValuesTest; // for test
        [SerializeField] private int[] _phLevelValuesTest; // for test
        [SerializeField] private int[] _humusValuesTest; // for test

        public IObservable<Unit> ConfirmButtonClick => _confirmButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        protected override void DoAwake()
        {
            UpdateTemperatureValues();
            UpdateHumidityValues();
            UpdatePhLevelValues();
            UpdateHumusValues();
        }

        private void UpdateTemperatureValues()
        {
            _rangeSlider.CalculateSliderPosition(_temperatureSlider, _temperatureValuesTest[0], _temperatureValuesTest[1]);
            _temperatureMin.text = _temperatureValuesTest[0].ToString();
            _temperatureMax.text = _temperatureValuesTest[1].ToString();
        }

        private void UpdateHumidityValues()
        {
            _rangeSlider.CalculateSliderPosition(_humididtySlider, _humididtyValuesTest[0], _humididtyValuesTest[1]);
            _humididtyMin.text = _humididtyValuesTest[0].ToString();
            _humididtyMax.text = _humididtyValuesTest[1].ToString();
        }

        private void UpdatePhLevelValues()
        {
            _rangeSlider.CalculateSliderPosition(_phLevelSlider, _phLevelValuesTest[0], _phLevelValuesTest[1]);
            _phLevelMin.text = _phLevelValuesTest[0].ToString();
            _phLevelMax.text = _phLevelValuesTest[1].ToString();
        }

        private void UpdateHumusValues()
        {
            _rangeSlider.CalculateSliderPosition(_humusSlider, _humusValuesTest[0], _humusValuesTest[1]);
            //_humusMin.text = _temperatureValuesTest[0].ToString();
            _humusMax.text = _humusValuesTest[1].ToString();
        }
    }
}