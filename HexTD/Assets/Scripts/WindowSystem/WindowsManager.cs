using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.EventSystems;
using WindowSystem.Controller;

namespace WindowSystem
{
	public class WindowsManager : IWindowsManager
	{
		private readonly WindowControllerFactory windowControllerFactory;
		private readonly EventSystem eventSystem;

		public ReactiveProperty<bool> IsScreenOverlapped { get; }

		public WindowsManager(
			WindowControllerFactory windowControllerFactory,
			EventSystem eventSystem)
		{
			this.windowControllerFactory = windowControllerFactory;
			this.eventSystem = eventSystem;

			IsScreenOverlapped = new ReactiveProperty<bool>(false);
		}

		public async UniTask<TWindowController> OpenAsync<TWindowController>(bool animated = true, params object[] args)
			where TWindowController : class, IWindowController, IWindowLoader
		{
			var controller = await InternalOpenWindowAsync<TWindowController>(animated, args);

			IsScreenOverlapped.Value = HasOpenedWindowsThatOverlapScreen();

			return controller;
		}

		public UniTask<TWindowController> OpenSingleAsync<TWindowController>(bool animated = true, params object[] args)
			where TWindowController : class, IWindowController, IWindowLoader
		{
			if (IsOpen<TWindowController>())
				return UniTask.FromResult(GetOpened<TWindowController>());

			return OpenAsync<TWindowController>(animated, args);
		}

		public bool IsOpen<TWindowController>() where TWindowController : class, IWindowController =>
			WindowsStorage.HasOpenedController<TWindowController>();

		public bool HasOpenedWindowsThatOverlapScreen() =>
			WindowsStorage.HasOpenedWindowsThatOverlapScreen();

		public TWindowController GetOpened<TWindowController>()
			where TWindowController : class, IWindowController, IWindowLoader
		{
			if (!IsOpen<TWindowController>())
				throw new IndexOutOfRangeException(
					$"Can't find opened window controller with type {typeof(TWindowController)}");

			return WindowsStorage.GetOpenedController<TWindowController>();
		}

		public UniTask CloseAsync(IWindowController windowController, bool animated = true)
		{
			var closeTask = InternalCloseAsync(windowController, animated)
				.ContinueWith(() => WindowsStorage.RemoveOpenedController(windowController));

			IsScreenOverlapped.Value = HasOpenedWindowsThatOverlapScreen();

			return closeTask;
		}

		private async UniTask<TWindowController> InternalOpenWindowAsync<TWindowController>(bool animated,
			object[] args)
			where TWindowController : class, IWindowController, IWindowLoader
		{
			var stopwatch = Stopwatch.StartNew();
			eventSystem.enabled = false;
			var controller = windowControllerFactory.Create<TWindowController>(args);
			var loadingResult = await controller.LoadWindowAsync();
			var controllerName = controller.GetType().Name;

			if (!loadingResult)
				throw new Exception($"Window {controllerName} loading failed!");

			stopwatch.Stop();
//            Log.Debug(LogTag.UI, $"{controllerName}. loading time {stopwatch.ElapsedMilliseconds}", this);
			UnityEngine.Debug.Log($"{controllerName}. loading time {stopwatch.ElapsedMilliseconds}");
			controller.Initialize();
			WindowsStorage.AddOpenedController(controller);

			await controller.ShowWindowAsync(animated);
			eventSystem.enabled = true;
			return controller;
		}


		private UniTask InternalCloseAsync(IWindowController windowController, bool animated = true)
		{
			if (eventSystem == null)
			{
				windowController.Dispose();
				return UniTask.CompletedTask;
			}

			eventSystem.enabled = false;

			return windowController.HideWindowAsync(animated).ContinueWith(() =>
			{
				eventSystem.enabled = true;
				windowController.Dispose();
			});
		}
	}

	public struct DialogChooseData
	{
		public bool IsForced { get; set; }
		public int SelectedVariant { get; set; }

		public static DialogChooseData Empty => new DialogChooseData();
	}
}