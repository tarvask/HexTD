using Match;
using UniRx;
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
			
			//todo #985621
			Container.Bind(typeof(IMessageBroker), typeof(IMessagePublisher), typeof(IMessageReceiver))
				.FromInstance(MessageBroker.Default);
//			Container.Bind<IMessageReceiver>().FromSubContainerResolve().ByInstaller<MatchInstaller>();
		}
	}
}