using System.Collections.Generic;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Tools;
using UniRx;

namespace Match.Windows.MainMenu
{
    public class PlayerHandTowerSelectionWindowController : BaseDisposable
    {
        public struct Context
        {
            public PlayerHandTowerSelectionWindowView View { get; }
            public ReactiveCommand<TowerInHandChangeParameters> TowerSlotSelected { get; }

            public Context(PlayerHandTowerSelectionWindowView view,
                ReactiveCommand<TowerInHandChangeParameters> towerSlotSelected)
            {
                View = view;
                TowerSlotSelected = towerSlotSelected;
            }
        }

        private readonly Context _context;

        public PlayerHandTowerSelectionWindowController(Context context)
        {
            _context = context;
            
            _context.View.BackButton.onClick.AddListener(Hide);
        }

        public void Show(List<TowerConfigNew> currentHand, byte currentHandIndex, TowerConfigNew selectedTowerConfig)
        {
            _context.View.gameObject.SetActive(true);
            
            // set up selected tower
            _context.View.SelectedTowerItem.SetData(selectedTowerConfig.RegularParameters);

            // set up towers in slots
            for (byte itemIndex = 0; itemIndex < _context.View.TowerSlots.Length; itemIndex++)
            {
                _context.View.TowerSlots[itemIndex].SetData(currentHand[itemIndex]?.RegularParameters);
            }

            for (byte slotIndex = 0; slotIndex < _context.View.TowerSlots.Length; slotIndex++)
            {
                byte index = slotIndex;
                _context.View.TowerSlots[slotIndex].SelectButton.onClick.AddListener(() =>
                    TowerSlotSelectedEventHandler(currentHand, currentHandIndex, index, selectedTowerConfig));
            }
        }

        public void Hide()
        {
            _context.View.gameObject.SetActive(false);
        }

        private void TowerSlotSelectedEventHandler(List<TowerConfigNew> currentHand, byte handIndex, byte slotIndex, TowerConfigNew towerConfig)
        {
            // hand cannot contain same towers
            if (currentHand.Find((handItem) =>
                handItem != null && handItem.RegularParameters.TowerType == towerConfig.RegularParameters.TowerType) != null)
                return;
            
            _context.View.TowerSlots[slotIndex].SetData(towerConfig.RegularParameters);
            _context.TowerSlotSelected.Execute(new TowerInHandChangeParameters(handIndex, slotIndex,
                towerConfig.RegularParameters.TowerType));
            Hide();
        }
    }
}