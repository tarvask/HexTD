using System;
using Tools;
using UniRx;
using UnityEngine;

namespace BuffLogic
{
    public class TimerBuffCondition : BaseDisposable, IBuffCondition
    {
        private readonly IReactiveProperty<float> _timerProperty;

        public TimerBuffCondition(float timerDuration)
        {
            _timerProperty = new ReactiveProperty<float>(timerDuration);
        }

        public void Subscribe(Action<float> onChangeAction)
        {
            AddDisposable(_timerProperty.Subscribe(onChangeAction));
        }
        
        public bool Invoke()
        {
            _timerProperty.Value -= Time.deltaTime;
            return _timerProperty.Value <= 0f;
        }
    }
}