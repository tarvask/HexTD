using Match;
using Match.Installers;
using UnityEngine;
using Zenject;

namespace MatchStarter
{
	public class MatchStarterInstaller : MonoInstaller
	{
		[SerializeField] private MatchView matchViewPrefab;

		public override void InstallBindings()
		{
			Container.BindFactory<MatchController.Context, MatchController, MatchController.Factory>()
				.FromSubContainerResolve()
				.ByNewContextPrefab<MatchInstaller>(matchViewPrefab)
				.UnderTransform(transform)
				.AsSingle();
		}
	}
}