using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using Zenject;

namespace WindowSystem.View.Factories.Addressable
{
	[CreateAssetMenu(fileName = "AddressableWindowViewContainer",
		menuName = "Game/Window/AddressableWindowViewContainer")]
	public class AddressableWindowViewContainer : ScriptableObject, IInitializable
	{
		[SerializeField] private List<WindowViewAsset> windowAssets;

		private Dictionary<Type, AssetReferenceInfo> _windowReferences;
		public IReadOnlyList<WindowViewAsset> WindowAssets => windowAssets;


		public void Initialize()
		{
			_windowReferences = new Dictionary<Type, AssetReferenceInfo>();

			foreach (var windowAsset in windowAssets)
			{
				var type = Type.GetType(windowAsset.viewType);
				if (type == null)
					throw new Exception($"Can't restore view with type {windowAsset.viewType}");

				_windowReferences[type] = new AssetReferenceInfo(windowAsset.assetReference, windowAsset.cacheInMemory);
			}
		}

		public AssetReferenceInfo GetReferenceInfo<TWindowView>() where TWindowView : WindowViewBase =>
			GetReferenceInfo(typeof(TWindowView));

		public AssetReferenceInfo GetReferenceInfo(Type type)
		{
			Assert.That(_windowReferences.ContainsKey(type), $"!windowReferences.ContainsKey {type}");
			return _windowReferences[type];
		}
	}
}