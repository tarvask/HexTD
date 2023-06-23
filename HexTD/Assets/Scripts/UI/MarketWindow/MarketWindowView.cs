using System;
using UI.CropItem;
using UI.GreenhouseItem;
using UI.OrderItem;
using UI.PlotItem;
using UI.SeedItem;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.ShopWindow
{
    public class MarketWindowView : WindowViewBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _cropsFilterButton;
        [SerializeField] private Button _plotsFilterButton;
        [SerializeField] private Button _seedsFilterButton;
        [SerializeField] private Button _greenhousesFilterButton;


        [Header("Roots")]
        [field: SerializeField] public Transform CropItemsRoot;
        [field: SerializeField] public Transform PlotItemsRoot;
        [field: SerializeField] public Transform SeedsItemsRoot;
        [field: SerializeField] public Transform GreenhouseItemsRoot;
        [field: SerializeField] public Transform OrderItemsRoot;

        [Header("Vies")]
        [field: SerializeField] public CropItemView CropItemView;
        [field: SerializeField] public PlotItemView PlotItemView;
        [field:SerializeField] public SeedsItemView SeedsItemView;
        [field: SerializeField] public GreenhouseItemView GreenhouseItemView;
        [field: SerializeField] public OrderItemView OrderItemView;


        public IObservable<Unit> CloseButtonClick => _closeButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> CropsFilterButtonClick => _cropsFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> PlotsFilterButtonClick => _plotsFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> SeedsFilterButtonClick => _seedsFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> GreenhousesFilterButtonClick => _greenhousesFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);
    }
}