using System;
using Addressables;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Extensions
{
	[Obsolete]
	public static class ZenjectExtensions
	{
		public static ConditionCopyNonLazyBinder BindFactoryFromAddressableAsset<TContract, TFactory>(
			this DiContainer container,
			AddressableAssetPreloader assetPreloader,
			AssetReference assetReference) where TFactory : PlaceholderFactory<TContract>
		{
			assetPreloader.RegisterReference(assetReference);
			return container
				.BindFactory<TContract, TFactory>()
				.FromMethod(c => Instantiate<TContract>(c, assetReference));
		}

		public static ConditionCopyNonLazyBinder BindFactoryFromAddressableAsset<TParam1, TContract, TFactory>(
			this DiContainer container,
			AddressableAssetPreloader assetPreloader,
			AssetReference assetReference) where TFactory : PlaceholderFactory<TParam1, TContract>
		{
			assetPreloader.RegisterReference(assetReference);
			return container
				.BindFactory<TParam1, TContract, TFactory>()
				.FromMethod((c, p1) => Instantiate<TContract>(c, assetReference, p1));
		}

		public static ConditionCopyNonLazyBinder BindFactoryFromAddressableAsset<TParam1, TParam2, TContract, TFactory>(
			this DiContainer container,
			AddressableAssetPreloader assetPreloader,
			AssetReference assetReference) where TFactory : PlaceholderFactory<TParam1, TParam2, TContract>
		{
			assetPreloader.RegisterReference(assetReference);
			return container
				.BindFactory<TParam1, TParam2, TContract, TFactory>()
				.FromMethod((c, p1, p2) => Instantiate<TContract>(c, assetReference, p1, p2));
		}

		public static ConditionCopyNonLazyBinder BindFactoryFromAddressableAsset<TParam1, TParam2, TParam3, TContract,
			TFactory>(
			this DiContainer container,
			AddressableAssetPreloader assetPreloader,
			AssetReference assetReference) where TFactory : PlaceholderFactory<TParam1, TParam2, TParam3, TContract>
		{
			assetPreloader.RegisterReference(assetReference);
			return container
				.BindFactory<TParam1, TParam2, TParam3, TContract, TFactory>()
				.FromMethod((c, p1, p2, p3) => Instantiate<TContract>(c, assetReference, p1, p2, p3));
		}

		private static T Instantiate<T>(IInstantiator container, AssetReference assetReference, params object[] args)
		{
			return container.InstantiatePrefabForComponent<T>(assetReference.Asset, args);
		}
	}
}