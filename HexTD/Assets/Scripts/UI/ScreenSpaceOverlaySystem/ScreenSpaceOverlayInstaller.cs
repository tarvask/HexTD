using UnityEngine;
using Zenject;

namespace UI.ScreenSpaceOverlaySystem
{
	public class ScreenSpaceOverlayInstaller : MonoInstaller
	{
		[SerializeField] private Transform poolParent;
		[SerializeField] private TargetObjectInfoPanelView targetObjectInfoPanelPrefab;

		public override void InstallBindings()
		{
			Container.Bind<ScreenSpaceOverlayController>().FromNew().AsSingle();

			Container
				.BindFactory<TargetObjectInfoPanelView.Context, TargetObjectInfoPanelView,
					TargetObjectInfoPanelView.Factory>()
				.FromMonoPoolableMemoryPool(x => x.WithInitialSize(40)
					.FromComponentInNewPrefab(targetObjectInfoPanelPrefab)
					.UnderTransform(poolParent));
		}
	}
}