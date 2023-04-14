using System;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows.Tower
{
    public class TowerItemView : BaseMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI towerNameLabel;
        [SerializeField] private TextMeshProUGUI towerPriceLabel;
        [SerializeField] private Image towerIcon;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject towerDisabledCover;
        [SerializeField] private Color[] raceColors;

        // model stuff
        private TowerShortParams _towerShortParams;
        private int _towerPriceInSilver;
        public TowerShortParams TowerShortParams => _towerShortParams;

        private Action _onBuyButtonClickAction;

        public void Init(TowerConfigNew config, Action onBuyButtonClickAction)
        {
            //Init(config, config.Levels[0].LevelRegularParams.Data.Price, onBuyButtonClickAction);
        }

        public void Init(TowerConfig config, int price, Action onBuyButtonClickAction)
        {
            _towerShortParams = new TowerShortParams(config.Parameters.RegularParameters.Data.TowerType, 1);
            _towerPriceInSilver = price;
            
            towerNameLabel.text = config.Parameters.RegularParameters.Data.TowerName;
            towerPriceLabel.text = $"{_towerPriceInSilver}";
            towerIcon.color = raceColors[(int) config.Parameters.RegularParameters.Data.RaceType];
            towerDisabledCover.SetActive(false);

            _onBuyButtonClickAction = onBuyButtonClickAction;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButtonClick);
            
            gameObject.SetActive(true);
        }

        public void Refresh(int currentSilverCoinsCount)
        {
            buyButton.interactable = currentSilverCoinsCount >= _towerPriceInSilver;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBuyButtonClick()
        {
            _onBuyButtonClickAction.Invoke();
        }
    }
}