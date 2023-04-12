using WindowSystem.Controller;

namespace UI.MatchInfoRoundStartInfo
{
	public class MatchRoundStartInfoWindowController : LoadableWindowController<MatchRoundStartInfoWindowView>
	{
		private MatchRoundStartInfoWindowView _viewOverride;

		public MatchRoundStartInfoWindowController(
			MatchRoundStartInfoWindowView view)
		{
			_viewOverride = view;
		}
	}
}