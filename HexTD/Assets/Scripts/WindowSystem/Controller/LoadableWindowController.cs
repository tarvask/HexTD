using Cysharp.Threading.Tasks;
using WindowSystem.View;
using Zenject;

namespace WindowSystem.Controller
{
	public abstract class LoadableWindowController<TWindowView> : WindowControllerBase<TWindowView>, IWindowLoader
		where TWindowView : WindowViewBase
	{
		private IWindowViewFactory _viewFactory;

		[Inject]
		private void InjectViewFactory(IWindowViewFactory viewFactory)
		{
			_viewFactory = viewFactory;
		}

		protected virtual UniTask<bool> DoLoadAsync() => UniTask.FromResult(true);

		async UniTask<bool> IWindowLoader.LoadWindowAsync()
		{
			var viewLoadOperation = await _viewFactory.CreateAsync<TWindowView>();

			if (viewLoadOperation.IsCompleted)
				SetView(viewLoadOperation.Result);

			var result = await DoLoadAsync();
			await UniTask.WaitForEndOfFrame();
			return viewLoadOperation.IsCompleted && result;
		}

		protected sealed override void OnViewDidHidden() => _viewFactory.Release(View);
	}
}