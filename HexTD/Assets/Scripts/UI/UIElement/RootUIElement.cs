using System.Linq;
using Cysharp.Threading.Tasks;

namespace UI.UIElement
{
	public abstract class RootUIElement : UIElement
	{
		protected virtual async void Start() => await AppearAsync();

		protected override UniTask AppearInternalAsync()
		{
			var tasks = GetComponentsInChildren<Widget>()
				.Select(w => w.AppearAsync())
				.Append(base.AppearInternalAsync());
			return UniTask.WhenAll(tasks);
		}

		protected override UniTask DisappearInternalAsync()
		{
			var tasks = GetComponentsInChildren<Widget>()
				.Select(w => w.DisappearAsync())
				.Append(base.DisappearInternalAsync());
			return UniTask.WhenAll(tasks);
		}
	}
}