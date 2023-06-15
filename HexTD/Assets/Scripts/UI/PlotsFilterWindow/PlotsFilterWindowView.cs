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
        [SerializeField] private Slider _humusValue;
        [SerializeField] private TextMeshProUGUI _humusValueText;

        public IObservable<Unit> ConfirmButtonClick => _confirmButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public void OnHumusValueChange()
        {
            _humusValueText.text = Mathf.RoundToInt(_humusValue.value).ToString();
        }
    }
}