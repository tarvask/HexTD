using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SingleSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _value;

    public float Value
    {
        get { return _slider.value; }
        set
        {
            _slider.value = value;
            _slider.onValueChanged.Invoke(_slider.value);
        }
    }

    private void Start()
    {
        UpdateLabel();
    }

    public void SetSliderValue(float value, UnityAction<float> valueChanged)
    {
        _slider.value = value;
        _slider.onValueChanged.AddListener(SliderOnValueChanged);
        _slider.onValueChanged.AddListener(valueChanged);
    }

    private void SliderOnValueChanged(float arg0)
    {
        UpdateLabel();
    }

    protected virtual void UpdateLabel()
    {
        _value.text = Mathf.RoundToInt(Value).ToString();
    }
}
