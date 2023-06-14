using Cysharp.Threading.Tasks;
using Extensions;
using UI.CropsFilterWindow;
using UI.GreenhousesFilterWindow;
using UI.OrderItem;
using UI.PlotsFilterWindow;
using UI.SeedsFilterWindow;
using UI.SeedsInfoSellWindow;
using UI.SeedItem;
using UI.Tools;
using UniRx;
using WindowSystem.Controller;
using UI.CropItem;
using UI.PlotItem;
using UI.GreenhouseItem;
using UI.SeedsInfoBuyWindow;

namespace UI.ShopWindow
{
    public class MarketWindowController : LoadableWindowController<MarketWindowView>
    {
        private UiElementListPool<CropItemView> _cropsItemViews;
        private UiElementListPool<PlotItemView> _plotsItemViews;
        private UiElementListPool<SeedsItemView> _seedsItemViews;
        private UiElementListPool<GreenhouseItemView> _greenhousesItemViews;
        private UiElementListPool<OrderItemView> _orderItemViews;

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

            _cropsItemViews = new UiElementListPool<CropItemView>(View.CropItemView, View.CropItemsRoot);
            _plotsItemViews = new UiElementListPool<PlotItemView>(View.PlotItemView, View.PlotItemsRoot);
            _seedsItemViews = new UiElementListPool<SeedsItemView>(View.SeedsItemView, View.SeedsItemsRoot);
            _greenhousesItemViews = new UiElementListPool<GreenhouseItemView>(View.GreenhouseItemView, View.GreenhouseItemsRoot);

            _orderItemViews = new UiElementListPool<OrderItemView>(View.OrderItemView, View.OrderItemsRoot);

            // temporary solution, cause number of elements unknown
            for (int i = 0; i < 18; i++)
            {
                CreateCropItem();
                CreatePlotItem();
                CreateSeedsItem();
                CreateGreenhouseItem();
                CreateOrderItem();
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

        private void OpenSeedsSellWindow()
        {
            WindowsManager.OpenAsync<SeedsInfoSellWindowController>();
        }

        private void OpenSeedsBuyWindow()
        {
            WindowsManager.OpenAsync<SeedsInfoBuyWindowController>();
        }

        private void CreateCropItem()
        {
            CropItemView cropItemView = _cropsItemViews.GetElement();
        }

        private void CreatePlotItem()
        {
            PlotItemView plotItemView = _plotsItemViews.GetElement();
        }

        private void CreateSeedsItem()
        {
            SeedsItemView seedItemView = _seedsItemViews.GetElement();
            seedItemView.InfoButton.onClick.AddListener(OpenSeedsBuyWindow);
        }

        private void CreateGreenhouseItem()
        {
            GreenhouseItemView greenhouseItemView = _greenhousesItemViews.GetElement();
        }
        private void CreateOrderItem()
        {
            OrderItemView orderItemView  = _orderItemViews.GetElement();
            orderItemView.CancelButton.onClick.AddListener(OpenSeedsSellWindow);
        }
    }
}