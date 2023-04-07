using Zenject;

namespace WindowSystem.Controller
{
	public class WindowControllerFactory
	{
		private readonly IInstantiator _instantiator;

		public WindowControllerFactory(IInstantiator instantiator)
		{
			_instantiator = instantiator;
		}

		public TWindowController Create<TWindowController>(params object[] args)
			where TWindowController : IWindowController =>
			_instantiator.Instantiate<TWindowController>(args);
	}
}