using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Addressables
{
	public interface IAddressableAssetsReferencesProvider
	{
		IReadOnlyList<AssetReferenceGameObject> GetAssetsReferences(List<string> ids);
	}
}