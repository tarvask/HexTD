using Addressables;
using UnityEngine;
using Zenject;

namespace UI
{
	[CreateAssetMenu(fileName = "UI", menuName = "Installers/UI")]
	public class UIInstaller : ScriptableObjectInstaller<UIInstaller>
	{
//        [Header("Widgets")]
//        [SerializeField] private AssetReferenceGameObject questResourceItemViewReference;

//        [Header("Panels")]
//        [SerializeField] private AssetReferenceGameObject restPanelReference;

//        [Header("Dialogs(Windows)")]
//        [SerializeField] private AssetReferenceGameObject inAppTipWindowReference;

		public override void InstallBindings()
		{
			var assetProvider = new AddressableAssetPreloader();

			Container
				.Bind<AddressableAssetPreloader>()
				.FromInstance(assetProvider)
				.AsSingle();

			// Widgets
//            Container.BindFactoryFromAddressableAsset<ItemView, ItemView.QuestPanelFactory>(assetProvider, questResourceItemViewReference);

			// Panels
//            Container.BindFactoryFromAddressableAsset<string, RestPanel, RestPanel.Factory>(assetProvider, restPanelReference);

			// Dialogs - Windows
//            Container.BindFactoryFromAddressableAsset<InAppTipData, InAppTipView, InAppTipView.Factory>(assetProvider, inAppTipWindowReference);
		}
	}
}