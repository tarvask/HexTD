using UniRx;
using Zenject;

namespace Match.Installers
{
	public class MatchInstaller: MonoInstaller
	{
		public override void InstallBindings()
		{
			//todo #985621
//			Container.Bind(typeof(IMessageBroker), typeof(IMessagePublisher), typeof(IMessageReceiver))
//				.FromInstance(MessageBroker.Default);
		}
	}
}