using Tools;
using Tools.Interfaces;

namespace Match.Windows
{
    public class MatchInfoRoundStartInfoPanelController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public MatchInfoRoundStartInfoPanelView View { get; }

            public Context(MatchInfoRoundStartInfoPanelView view)
            {
                View = view;
            }
        }

        private readonly Context _context;
        private bool _isShown;
        private float _currentTimer;

        public bool IsShown => _isShown;

        public MatchInfoRoundStartInfoPanelController(Context context)
        {
            _context = context;
        }
        
        public void Show(int roundNumber, float showDuration)
        {
            _isShown = true;
            _currentTimer = showDuration;
            _context.View.RoundNumberText.text = $"Round\n{roundNumber}";
            
            _context.View.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _isShown = false;
            _context.View.gameObject.SetActive(false);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _currentTimer -= frameLength;

            if (_currentTimer < 0)
                Hide();
        }
    }
}