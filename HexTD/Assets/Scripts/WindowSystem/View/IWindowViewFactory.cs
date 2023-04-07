using Cysharp.Threading.Tasks;
using Tools;

namespace WindowSystem.View
{
	public interface IWindowViewFactory
	{
		UniTask<OperationResult<TWindowView>> CreateAsync<TWindowView>()
			where TWindowView : WindowViewBase;

		void Release(WindowViewBase windowView);
	}
}