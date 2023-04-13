using WindowSystem.Controller;

namespace UI.OverlayWindow
{
	public class EnemyFieldViewWindowController : LoadableWindowController<EnemyFieldViewWindowView>
	{
		private EnemyFieldViewWindowView _viewOverride;

		public EnemyFieldViewWindowController(
			EnemyFieldViewWindowView view)
		{
			_viewOverride = view;
		}
	}
}