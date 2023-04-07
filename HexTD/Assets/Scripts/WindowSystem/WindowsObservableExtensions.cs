using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UI.UIElement;
using UniRx;
using WindowSystem.Controller;
using WindowSystem.View;

namespace WindowSystem
{
	public static class WindowsObservableExtensions
	{
		public static IObservable<T> WhereAppeared<T>(this IObservable<T> source, WindowViewBase view) =>
			source.Where(_ => view.State == UIElementState.Appeared);

		public static UniTask WaitUntilClosedAsync(this IWindowController controller,
			CancellationToken token = default)
		{
			return UniTask.WaitUntil(() => !WindowsStorage.HasOpenedController(controller),
					PlayerLoopTiming.FixedUpdate, token)
				.SuppressCancellationThrow();
		}

		public static async UniTask WaitUntilClosedAsync<T>(this UniTask<T> loadingControllerTask,
			CancellationToken token = default) where T : class, IWindowController
		{
			var controller = await loadingControllerTask;
			await controller.WaitUntilClosedAsync(token);
		}

		public static async UniTask<T> WaitUntilOpenedAsync<T>(CancellationToken token = default)
			where T : class, IWindowController
		{
			await UniTask.WaitUntil(WindowsStorage.HasOpenedController<T>, PlayerLoopTiming.FixedUpdate, token)
				.SuppressCancellationThrow();
			return WindowsStorage.GetOpenedController<T>();
		}
	}
}