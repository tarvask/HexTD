using Zenject;

namespace SceneManagement
{
	public class SceneManagementInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<SceneLoader>().AsSingle();
		}
	}
}