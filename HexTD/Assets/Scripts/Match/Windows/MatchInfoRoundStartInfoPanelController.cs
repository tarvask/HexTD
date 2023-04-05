using Tools;
using Tools.Interfaces;
using UnityEngine.UI;

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
        
        public void Show(int roundNumber, Image[] waveElementsSprites, Image[] reinforcementItemsSprites,
            float showDuration)
        {
            _isShown = true;
            _currentTimer = showDuration;
            _context.View.RoundNumberText.text = $"Round\n{roundNumber}";

            for (int waveElementIndex = 0; waveElementIndex < waveElementsSprites.Length; waveElementIndex++)
                _context.View.WaveElements[waveElementIndex].sprite = waveElementsSprites[waveElementIndex].sprite;

            for (int reinforcementItemIndex = 0; reinforcementItemIndex < reinforcementItemsSprites.Length; reinforcementItemIndex++)
                _context.View.ReinforcementsItems[reinforcementItemIndex].sprite = reinforcementItemsSprites[reinforcementItemIndex].sprite;
            
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