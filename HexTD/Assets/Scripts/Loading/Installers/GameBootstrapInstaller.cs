using Zenject;

namespace Loading.Installers
{
	public class GameBootstrapInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<GameBootstrap>().AsSingle();
		}
	}
}