using UnityEngine;
using Zenject;

namespace Installers
{
	public class GameInstaller: MonoInstaller
	{
		[SerializeField] private bool isMultiplayer;

		public override void InstallBindings()
		{
			Container.Bind<bool>().WithId("isMultiplayer").FromInstance(isMultiplayer).AsSingle();
		}
	}
}