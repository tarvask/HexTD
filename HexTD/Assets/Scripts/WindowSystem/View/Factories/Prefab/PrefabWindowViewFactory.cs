using Cysharp.Threading.Tasks;
using Tools;

namespace WindowSystem.View.Factories.Prefab
{
	public class PrefabWindowViewFactory : IWindowViewFactory
	{
		public UniTask<OperationResult<TWindowView>> CreateAsync<TWindowView>() where TWindowView : WindowViewBase
		{
			throw new System.NotImplementedException("Prefab window loading is not implemented");
		}

		public void Release(WindowViewBase windowView)
		{
			throw new System.NotImplementedException("Prefab window loading is not implemented");
		}
	}
}