using Match;
using UnityEngine;
using Zenject;

namespace Game
{
	public class GameInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind(typeof(MatchSettingsProvider))
				.To<MatchSettingsProvider>()
				.AsSingle();

			Container
				.Bind(typeof(GameController))
				.To<GameController>()
				.AsSingle();
		}
	}
}