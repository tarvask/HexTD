using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UI.UIElement;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Extensions
{
	public static class ObservableExtensions
	{
		public static IDisposable Subscribe<T>(this IObservable<T> source, Action onNext)
		{
			return source.Subscribe(_ => onNext());
		}

		public static IDisposable Subscribe<T>(this IObservable<T> source, Func<UniTaskVoid> onNext)
		{
			return source.Subscribe(_ => onNext().Forget());
		}

		public static IDisposable Subscribe<T>(this IObservable<T> source, Func<T, UniTaskVoid> onNext)
		{
			return source.Subscribe(value => onNext(value).Forget());
		}

		public static IDisposable Subscribe<T>(this IObservable<T> source, Func<UniTask> onNext)
		{
			return source.Subscribe(_ => onNext().Forget());
		}

		public static IDisposable Subscribe<T>(this IObservable<T> source, Func<T, UniTask> onNext)
		{
			return source.Subscribe(value => onNext(value).Forget());
		}

		public static IDisposable SubscribeSuppressed<T>(this IObservable<T> source, Func<UniTask> onNext)
		{
			return source.Subscribe(_ => onNext().SuppressCancellationThrow().Forget());
		}

		public static IDisposable SubscribeSuppressed<T>(this IObservable<T> source, Func<T, UniTask> onNext)
		{
			return source.Subscribe(value => onNext(value).SuppressCancellationThrow().Forget());
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<CancellationToken, UniTask> onNext)
		{
			return SubscribeCancelable(source, onNext, CancellationToken.None);
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<T, CancellationToken, UniTask> onNext)
		{
			return SubscribeCancelable(source, onNext, CancellationToken.None);
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<CancellationToken, UniTask> onNext, Component component)
		{
			return SubscribeCancelable(source, onNext, component.gameObject);
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<T, CancellationToken, UniTask> onNext, Component component)
		{
			return SubscribeCancelable(source, onNext, component.gameObject);
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<CancellationToken, UniTask> onNext, GameObject gameObject)
		{
			return SubscribeCancelable(source, onNext, gameObject.GetCancellationTokenOnDestroy());
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<T, CancellationToken, UniTask> onNext, GameObject gameObject)
		{
			return SubscribeCancelable(source, onNext, gameObject.GetCancellationTokenOnDestroy());
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<CancellationToken, UniTask> onNext, CancellationToken cancellationToken)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			return source.Subscribe(_ =>
			{
				cts.Cancel();
				cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				onNext(cts.Token).SuppressCancellationThrow().Forget();
			});
		}

		public static IDisposable SubscribeCancelable<T>(this IObservable<T> source,
			Func<T, CancellationToken, UniTask> onNext, CancellationToken cancellationToken)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			return source.Subscribe(value =>
			{
				cts.Cancel();
				cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				onNext(value, cts.Token).SuppressCancellationThrow().Forget();
			});
		}

		public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshPro text)
		{
			return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
		}

		public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text)
		{
			return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
		}

		public static IDisposable SubscribeToActive(this IObservable<bool> source, Component component)
		{
			return source.SubscribeToActive(component.gameObject);
		}

		public static IDisposable SubscribeToActive(this IObservable<bool> source, GameObject component)
		{
			return source.SubscribeWithState(component, (x, go) => go.SetActive(x));
		}

		public static IDisposable SubscribeToSprite(this IObservable<Sprite> source, Image image)
		{
			return source.SubscribeWithState(image, (x, i) => i.sprite = x);
		}

		public static IDisposable SubscribeToSprite(this IObservable<Sprite> source, SpriteRenderer image)
		{
			return source.SubscribeWithState(image, (x, i) => i.sprite = x);
		}

		public static IDisposable SubscribeDestroy<T>(this IObservable<T> source, Object obj)
		{
			return source.Subscribe(_ => Object.Destroy(obj));
		}

		public static IObservable<bool> Not(this IObservable<bool> source)
		{
			return source.Select(b => !b);
		}

		public static IDisposable SubscribeExecute<T>(
			this IObservable<T> source,
			IReactiveCommand<T> command)
		{
			return source.Subscribe(value => command.Execute(value));
		}

		public static IDisposable SubscribeToInteractable(this IObservable<bool> source, CanvasGroup canvasGroup)
		{
			return source.SubscribeWithState(canvasGroup, (x, s) => s.interactable = x);
		}

		public static IDisposable SubscribeToInteractable(this IObservable<bool> source, Button button)
		{
			return source.SubscribeWithState(button, (x, s) => s.interactable = x);
		}

		public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
		{
			return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
			{
				observer.OnNext(i.text);
				return i.onValueChanged.AsObservable().Subscribe(observer);
			});
		}

		public static IObservable<T> WhereAppeared<T>(this IObservable<T> source, UIElement element)
		{
			return source.Where(_ => element.State == UIElementState.Appeared);
		}

		public static IObservable<bool> WhereTrue(this IObservable<bool> source)
		{
			return source.Where(value => value);
		}

		public static void OnNext(this Subject<Unit> subject)
		{
			subject.OnNext(Unit.Default);
		}

		public static T AddTo<T>(this T disposable, CancellationTokenSource cancellationTokenSource)
			where T : IDisposable
		{
			return AddTo(disposable, cancellationTokenSource.Token);
		}

		public static T AddTo<T>(this T disposable, CancellationToken cancellationToken)
			where T : IDisposable
		{
			if (cancellationToken.IsCancellationRequested)
			{
				disposable.Dispose();
				return disposable;
			}

			cancellationToken.Register(disposable.Dispose);
			return disposable;
		}
	}
}