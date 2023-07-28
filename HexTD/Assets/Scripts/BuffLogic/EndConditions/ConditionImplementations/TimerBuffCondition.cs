using System;
using ExitGames.Client.Photon;
using Tools;
using UniRx;
using UnityEngine;

namespace BuffLogic
{
    public class TimerBuffCondition : BaseDisposable, IBuffCondition
    {
        private const string TimeName = "Time";
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
        
        public Hashtable ToNetwork()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add(TimeName, _timerProperty.Value);

            return hashtable;
        }
        
        public static object FromNetwork(Hashtable hashtable)
        {
            float time = (float)hashtable[TimeName];
            TimerBuffCondition timerBuffCondition = new TimerBuffCondition(time);

            return timerBuffCondition;
        }
    }
}