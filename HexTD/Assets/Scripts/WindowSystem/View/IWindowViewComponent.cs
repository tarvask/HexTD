using Cysharp.Threading.Tasks;

namespace WindowSystem.View
{
	public interface IWindowViewComponent
	{
		UniTask AppearAsync(bool animated = true);
		void Appeared();

		UniTask DisappearAsync(bool animated = true);
		void Disappeared();
	}
}