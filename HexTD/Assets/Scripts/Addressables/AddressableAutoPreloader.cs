using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Addressables
{
	public class AddressableAutoPreloader : MonoBehaviour
	{
		private AddressableAssetPreloader preloader;

		[Inject]
		private void Construct(AddressableAssetPreloader preloader)
		{
			this.preloader = preloader;
		}

		private void Start()
		{
			preloader.PreloadAssetsAsync().Forget();
		}
	}
}