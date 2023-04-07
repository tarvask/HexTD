using UnityEngine;
using Zenject;

namespace UI.OverlayHud
{
	[CreateAssetMenu(fileName = "OverlayInstaller", menuName = "Installers/OverlayInstaller")]
	public class OverlayUIInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private OverlayPanel overlayPanelPrefab;

		public override void InstallBindings()
		{
			Container.BindFactory<OverlayPanel, OverlayPanel.Factory>()
				.FromComponentInNewPrefab(overlayPanelPrefab)
				.AsSingle();

			Container.Bind<OverlayPanel>()
				.FromMethod(() => Container.Resolve<OverlayPanel.Factory>().Create())
				.AsSingle();
		}
	}
}