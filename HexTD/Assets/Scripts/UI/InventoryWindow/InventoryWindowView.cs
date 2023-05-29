using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WindowSystem;
using WindowSystem.View;

namespace UI.InventoryWindow
{
    public class InventoryWindowView : WindowViewBase
    {
        [SerializeField] private Button _closeButton;

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