using Cysharp.Threading.Tasks;
using Extensions;
using UI.CropsFilterWindow;
using UI.GreenhousesFilterWindow;
using UI.PlotsFilterWindow;
using UI.SeedsFilterWindow;
using UI.SeedsInfoWindow;
using UI.SeedsItem;
using UI.Tools;
using UniRx;
using WindowSystem.Controller;

namespace UI.ShopWindow
{
    public class MarketWindowController : LoadableWindowController<MarketWindowView>
    {
        private UiElementListPool<SeedsItemView> _seedsItemViews;

        protected override void DoInitialize()
        {
            View.CloseButtonClick
                .Subscribe(CloseWindow)
                .AddTo(View);

            View.SeedsFilterButtonClick
                .Subscribe(OpenSeedsFilterWindow)
                .AddTo(View);

            View.CropsFilterButtonClick
                .Subscribe(OpenCropsFilterWindow)
                .AddTo(View);

            View.PlotsFilterButtonClick
                .Subscribe(OpenPlotsFilterWindow)
                .AddTo(View);

            View.GreenhousesFilterButtonClick
                .Subscribe(OpenGreenhousesFilterWindow)
                .AddTo(View);

            _seedsItemViews = new UiElementListPool<SeedsItemView>(View.SeedsItemView,
                View.SeedsItemsRoot);

            // temporary solution, cause number of elements unknown
            for (int i = 0; i < 5; i++)
            {
                CreateSeedsItem();
            }
        }

        private void CloseWindow()
        {
            WindowsManager.CloseAsync(this).Forget();
        }

        private void OpenSeedsFilterWindow()
        {
            WindowsManager.OpenAsync<SeedsFilterWindowController>();
        }

        private void OpenCropsFilterWindow()
        {
            WindowsManager.OpenAsync<CropsFilterWindowController>();
        }

        private void OpenPlotsFilterWindow()
        {
            WindowsManager.OpenAsync<PlotsFilterWindowController>();
        }

        private void OpenGreenhousesFilterWindow()
        {
            WindowsManager.OpenAsync<GreenhousesFilterWindowController>();
        }

        private void OpenSeedsInfoWindow()
        {
            WindowsManager.OpenAsync<SeedsInfoWindowController>();
        }

        private void CreateSeedsItem()
        {
            SeedsItemView towerCardView = _seedsItemViews.GetElement();
            towerCardView.InfoButton.onClick.AddListener(OpenSeedsInfoWindow);
        }
    }
}