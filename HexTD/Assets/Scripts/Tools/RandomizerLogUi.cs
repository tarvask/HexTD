using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Tools
{
    public class RandomizerLogUi : BaseMonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI seedText;
        [SerializeField] private TextMeshProUGUI randomCallsCountText;
        [SerializeField] private TextMeshProUGUI lastIntText;
        [SerializeField] private TextMeshProUGUI lastFloatText;

        private IDisposable _randomCallsCountReactivePropertySubscription;
        private IDisposable _lastRandomIntValueReactivePropertySubscription;
        private IDisposable _lastRandomFloatValueReactivePropertySubscription;

        private void Awake()
        {
            if (!Randomizer.ShowLog)
            {
                gameObject.SetActive(false);
                return;
            }

            _randomCallsCountReactivePropertySubscription = Randomizer.RandomCallsCountReactiveProperty
                .ObserveEveryValueChanged(rp => rp.Value)
                .Subscribe(callsCount => randomCallsCountText.text = $"{callsCount}");
            _lastRandomIntValueReactivePropertySubscription = Randomizer.LastRandomIntValueReactiveProperty
                .ObserveEveryValueChanged(rp => rp.Value)
                .Subscribe(lastIntValue => lastIntText.text = $"{lastIntValue}");
            _lastRandomFloatValueReactivePropertySubscription = Randomizer.LastRandomFloatValueReactiveProperty
                .ObserveEveryValueChanged(rp => rp.Value)
                .Subscribe(lastFloatValue => lastFloatText.text = $"{lastFloatValue}");
            
            gameObject.SetActive(true);
        }

        protected override void OnDestroy()
        {
            _randomCallsCountReactivePropertySubscription?.Dispose();
            _lastRandomIntValueReactivePropertySubscription?.Dispose();
            _lastRandomFloatValueReactivePropertySubscription?.Dispose();
            
            base.OnDestroy();
        }
    }
}