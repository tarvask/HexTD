using MatchDataBase;
using UnityEngine;
using Zenject;

namespace Installers
{
	[CreateAssetMenu(fileName = "MatchDataBase", menuName = "Installers/MatchDataBase")]
	public class MatchDataBaseInstaller : ScriptableObjectInstaller<MatchDataBaseInstaller>
	{
		[SerializeField] private MatchDataBase.MatchDataBase.Settings matchDataBaseSettings;

		public override void InstallBindings()
		{
//			Container.Bind<MatchDataBase>().AsSingle().WithArguments(matchDataBaseSettings);
			Container.Bind<MatchStarterSettings>().FromInstance(matchDataBaseSettings.MatchStarterSettings).AsSingle();
		}
	}
}