using System;
using UniRx;
using UnityEngine;

namespace Tools
{
	internal class SimpleStopwatch : IObservable<int>
	{
		private Subject<int> _subject = new Subject<int>();

		private float _delay;
		private bool _isStarted;
		private float _elapsed;

		public SimpleStopwatch(float delay)
		{
			_delay = delay;
		}

		public IDisposable Subscribe(IObserver<int> observer) => _subject.Subscribe(observer);

		public void Start()
		{
//			Debug.Log($"{nameof(Start)}");
			_isStarted = true;
		}
            
		public void Stop()
		{
//			Debug.Log($"{nameof(Stop)}");
			_isStarted = false;
			Reset();
		}
            
		public void Reset()
		{
			_elapsed = 0.0f;
		}

		public void Update(float deltaTime)
		{
			if (!_isStarted)
			{
				return;
			}
                
			_elapsed += deltaTime;

			var cycles = Mathf.FloorToInt(_elapsed / _delay);
			if (cycles > 0)
			{
				_elapsed %= _delay;
				_subject.OnNext(cycles);
			}
		}
	}
}