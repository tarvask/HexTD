using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoubleSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SingleSlider _sliderMin;
    [SerializeField] private SingleSlider _sliderMax;
    [SerializeField] private RectTransform _fillArea;
    [SerializeField] private RectTransform _fillRect;

    [Header("Configuration")]
    [SerializeField] private float _minValue;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _initialMinValue;
    [SerializeField] private float _initialMaxValue;

    [Header("Events")]
    public UnityEvent<float, float> OnValueChanged;

    public float MinValue
    {
        get { return _sliderMin.Value; }
    }
    public float MaxValue
    {
        get { return _sliderMax.Value; }
    }

    private void Awake()
    {
        Setup(_minValue, _maxValue, _initialMinValue, _initialMaxValue);
    }

    public void Setup(float minValue, float maxValue, float initialMinValue, float initialMaxValue)
    {
        _minValue = minValue;
        _maxValue = maxValue;
        _initialMinValue = initialMinValue;
        _initialMaxValue = initialMaxValue;

        _sliderMin.SetSliderValue(_initialMinValue, MinValueChanged);
        _sliderMax.SetSliderValue(_initialMaxValue,MaxValueChanged);

        MinValueChanged(_initialMinValue);
        MaxValueChanged(_initialMaxValue);
    }

    private void MinValueChanged(float value)
    {
        float offset = ((MinValue - _minValue) / (_maxValue - _minValue)) * _fillArea.rect.width;

        _fillRect.offsetMin = new Vector2(offset, _fillRect.offsetMin.y);

        if ((MaxValue - value) < _minDistance)
        {
            _sliderMin.Value = MaxValue - _minDistance;
        }

        OnValueChanged.Invoke(MinValue, MaxValue);
    }
    private void MaxValueChanged(float value)
    {
        float offset = (1 - ((MaxValue - _minValue) / (_maxValue - _minValue))) * _fillArea.rect.width;

        _fillRect.offsetMax = new Vector2(-offset, _fillRect.offsetMax.y);

        if ((value - MinValue) < _minDistance)
        {
            _sliderMax.Value = MinValue + _minDistance;
        }

        OnValueChanged.Invoke(MinValue, MaxValue);
    }
}
