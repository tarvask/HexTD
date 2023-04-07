using Loading.Proxy;
using Zenject;

namespace Loading.Installers
{
	// Project context
	public class GameLoadingProxyInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<GameLoadingProxy>().AsSingle();
		}
	}
}