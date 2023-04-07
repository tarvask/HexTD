using Zenject;

namespace WindowSystem.Controller
{
	public class WindowControllerFactory
	{
		private readonly IInstantiator instantiator;

		public WindowControllerFactory(IInstantiator instantiator)
		{
			this.instantiator = instantiator;
		}

		public TWindowController Create<TWindowController>(params object[] args)
			where TWindowController : IWindowController =>
			instantiator.Instantiate<TWindowController>(args);
	}
}