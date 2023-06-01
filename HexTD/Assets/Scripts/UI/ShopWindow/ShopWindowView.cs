using System;
using UI.SeedsItem;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.ShopWindow
{
    public class ShopWindowView : WindowViewBase
    {
        [SerializeField] private Button _closeButton;

        [SerializeField] private Button _seedsTabButton;
        [SerializeField] private Button _cropsTabButton;
        [SerializeField] private Button _plotsTabButton;
        [SerializeField] private Button _greenhousesTabButton;

        [field:SerializeField] public Transform SeedsItemsRoot;
        [field:SerializeField] public SeedsItemView SeedsItemView;

        [SerializeField] private Button _seedsFilterButton;
        [SerializeField] private Button _plotsFilterButton;

        [SerializeField] private GameObject _seedsSelectedTab;
        [SerializeField] private GameObject _cropsSelectedTab;
        [SerializeField] private GameObject _plotsSelectedTab;
        [SerializeField] private GameObject _greenhousesSelectedTab;

        [SerializeField] private GameObject _seedsSelectedContent;
        [SerializeField] private GameObject _cropsSelectedContent;
        [SerializeField] private GameObject _plotsSelectedContent;
        [SerializeField] private GameObject _greenhousesSelectedContent;

        public IObservable<Unit> CloseButtonClick => _closeButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> SeedsFilterButtonClick => _seedsFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        public IObservable<Unit> PlotsFilterButtonClick => _plotsFilterButton
            .OnClickAsObservable()
            .WhereAppeared(this);

        protected override void DoAwake()
        {
            _seedsTabButton.onClick.AddListener(SelectSeedsTab);
            _cropsTabButton.onClick.AddListener(SelectCropsTab);
            _plotsTabButton.onClick.AddListener(SelectPlotsTab);
            _greenhousesTabButton.onClick.AddListener(SelectGreenhousesTab);
        }

        public void SelectSeedsTab()
        {
            _seedsSelectedTab.SetActive(true);
            _cropsSelectedTab.SetActive(false);
            _plotsSelectedTab.SetActive(false);
            _greenhousesSelectedTab.SetActive(false);

            _seedsSelectedContent.SetActive(true);
            _cropsSelectedContent.SetActive(false);
            _plotsSelectedContent.SetActive(false);
            _greenhousesSelectedContent.SetActive(false);
        }

        public void SelectCropsTab()
        {
            _seedsSelectedTab.SetActive(false);
            _cropsSelectedTab.SetActive(true);
            _plotsSelectedTab.SetActive(false);
            _greenhousesSelectedTab.SetActive(false);

            _seedsSelectedContent.SetActive(false);
            _cropsSelectedContent.SetActive(true);
            _plotsSelectedContent.SetActive(false);
            _greenhousesSelectedContent.SetActive(false);
        }

        public void SelectPlotsTab()
        {
            _seedsSelectedTab.SetActive(false);
            _cropsSelectedTab.SetActive(false);
            _plotsSelectedTab.SetActive(true);
            _greenhousesSelectedTab.SetActive(false);

            _seedsSelectedContent.SetActive(false);
            _cropsSelectedContent.SetActive(false);
            _plotsSelectedContent.SetActive(true);
            _greenhousesSelectedContent.SetActive(false);
        }

        public void SelectGreenhousesTab()
        {
            _seedsSelectedTab.SetActive(false);
            _cropsSelectedTab.SetActive(false);
            _plotsSelectedTab.SetActive(false);
            _greenhousesSelectedTab.SetActive(true);

            _seedsSelectedContent.SetActive(false);
            _cropsSelectedContent.SetActive(false);
            _plotsSelectedContent.SetActive(false);
            _greenhousesSelectedContent.SetActive(true);
        }
    }
}