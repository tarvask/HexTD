using Zenject;

namespace SceneManagement
{
	public class SceneLoaderInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<SceneLoader>().AsSingle();
		}
	}
}