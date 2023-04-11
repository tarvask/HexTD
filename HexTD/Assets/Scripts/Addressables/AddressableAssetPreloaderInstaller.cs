using UnityEngine;
using Zenject;

namespace Addressables
{
	[CreateAssetMenu(fileName = "AddressableAssetPreloader", menuName = "Installers/AddressableAssetPreloader")]
	public class AddressableAssetPreloaderInstaller : ScriptableObjectInstaller<AddressableAssetPreloaderInstaller>
	{
		public override void InstallBindings()
		{
			Container
				.Bind<AddressableAssetPreloader>()
				.FromInstance(new AddressableAssetPreloader())
				.AsSingle();
		}
	}
}