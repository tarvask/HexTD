using Cysharp.Threading.Tasks;
using WindowSystem.Controller;

namespace WindowSystem
{
	public static class WindowsManagerExtensions
	{
		public static UniTask CloseIfExists<TWindowController>(this IWindowsManager manager, bool animated = true)
			where TWindowController : class, IWindowController, IWindowLoader
		{
			return manager.IsOpen<TWindowController>()
				? manager.CloseAsync(manager.GetOpened<TWindowController>(), animated)
				: UniTask.CompletedTask;
		}

		public static void CloseAllWindowsImmediately(this IWindowsManager manager)
		{
			var allOpenedWindows = WindowsStorage.GetAllOpenedControllers();

			foreach (var windowController in allOpenedWindows)
			{
				manager.CloseAsync(windowController, false).Forget();
			}
		}

		public static bool TryGetOpened<TWindowController>(this IWindowsManager manager,
			out TWindowController controller)
			where TWindowController : class, IWindowController, IWindowLoader
		{
			var isOpen = manager.IsOpen<TWindowController>();
			controller = isOpen ? manager.GetOpened<TWindowController>() : null;
			return isOpen;
		}
	}
}