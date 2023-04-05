using System.Threading.Tasks;
using Tools;
using UniRx;

namespace Match.Windows
{
    public abstract class BaseWindowController : BaseDisposable
    {
        private const int WindowClosingDuration = 300;
        
        private readonly BaseWindowView _baseView;
        private readonly IReactiveProperty<int> _openWindowsCountReactiveProperty;
        private bool _isShown;

        public bool IsShown => _isShown;

        protected BaseWindowController(BaseWindowView baseView,
            IReactiveProperty<int> openWindowsCountReactiveProperty)
        {
            _baseView = baseView;
            _openWindowsCountReactiveProperty = openWindowsCountReactiveProperty;
        }
        
        public void ShowWindow()
        {
            if (_isShown)
                return;
            
            if (_baseView.IsBlockingClicksToField)
                _openWindowsCountReactiveProperty.Value++;
            
            _isShown = true;
            _baseView.Show();
        }

        protected void HideWindow()
        {
            if (!_isShown)
                return;
            
            _baseView.Hide();
            _isShown = false;

            Task.Run(async () =>
            {
                await Task.Delay(WindowClosingDuration);
                
                if (_baseView.IsBlockingClicksToField)
                    _openWindowsCountReactiveProperty.Value--;
            });
        }
    }
}