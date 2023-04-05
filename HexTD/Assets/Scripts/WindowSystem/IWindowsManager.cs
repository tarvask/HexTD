using Cysharp.Threading.Tasks;
using UniRx;
using WindowSystem.Controller;

namespace WindowSystem
{
	public interface IWindowsManager
	{
		ReactiveProperty<bool> IsScreenOverlapped { get; }

		UniTask<TWindowController> OpenAsync<TWindowController>(bool animated = true, params object[] args)
			where TWindowController : class, IWindowController, IWindowLoader;

		UniTask<TWindowController> OpenSingleAsync<TWindowController>(bool animated = true, params object[] args)
			where TWindowController : class, IWindowController, IWindowLoader;

		bool IsOpen<TWindowController>()
			where TWindowController : class, IWindowController;

		TWindowController GetOpened<TWindowController>()
			where TWindowController : class, IWindowController, IWindowLoader;

		UniTask CloseAsync(IWindowController windowController, bool animated = true);
	}
}