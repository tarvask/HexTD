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
        private int _towerPriceInCoins;
        public TowerShortParams TowerShortParams => _towerShortParams;

        private Action _onBuyButtonClickAction;

        public void Init(TowerConfigNew config, Action onBuyButtonClickAction)
        {
            Init(config, config.TowerLevelConfigs[TowerConfigNew.FirstTowerLevel].BuildPrice, onBuyButtonClickAction);
        }

        public void Init(TowerConfigNew config, int price, Action onBuyButtonClickAction)
        {
            _towerShortParams = new TowerShortParams(config.RegularParameters.TowerType, TowerConfigNew.FirstTowerLevel);
            _towerPriceInCoins = price;
            
            towerNameLabel.text = config.RegularParameters.TowerName;
            towerPriceLabel.text = $"{_towerPriceInCoins}";
            towerIcon.color = raceColors[(int) config.RegularParameters.RaceType];
            towerDisabledCover.SetActive(false);

            _onBuyButtonClickAction = onBuyButtonClickAction;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButtonClick);
            
            gameObject.SetActive(true);
        }

        public void Refresh(int currentCoinsCount)
        {
            buyButton.interactable = currentCoinsCount >= _towerPriceInCoins;
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