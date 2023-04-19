using System;
using Match.Windows;
using Match.Windows.Hand;
using Match.Windows.Tower;
using UnityEngine;

namespace Match
{
    [Serializable]
    public class MatchUiViewsCollection
    {
        [SerializeField] private MatchStartInfoWindowView matchStartInfoView;
        [SerializeField] private HandTowerSelectionView handTowerSelectionView;
        [SerializeField] private WaveStartInfoWindowView waveStartInfoWindowView;
        [SerializeField] private WinLoseWindowView winLoseWindowView;
        [SerializeField] private DisconnectBlockerWindowView disconnectBlockerWindowView;
        
        [SerializeField] private MatchInfoPanelView matchInfoPanelView;
        [SerializeField] private TowerSelectionWindowView towerSelectionWindowView;
        [SerializeField] private TowerManipulationWindowView towerManipulationWindowView;
        [SerializeField] private TowerInfoWindowView towerInfoWindowView;
        [SerializeField] private MobInfoWindowView mobInfoWindowView;

        public MatchStartInfoWindowView MatchStartInfoView => matchStartInfoView;
        public HandTowerSelectionView HandTowerSelectionView => handTowerSelectionView;
        public WaveStartInfoWindowView WaveStartInfoWindowView => waveStartInfoWindowView;
        public WinLoseWindowView WinLoseWindowView => winLoseWindowView;
        public DisconnectBlockerWindowView DisconnectBlockerWindowView => disconnectBlockerWindowView;

        public MatchInfoPanelView MatchInfoPanelView => matchInfoPanelView;
        public TowerSelectionWindowView TowerSelectionWindowView => towerSelectionWindowView;
        public TowerManipulationWindowView TowerManipulationWindowView => towerManipulationWindowView;
        public TowerInfoWindowView TowerInfoWindowView => towerInfoWindowView;
        public MobInfoWindowView MobInfoWindowView => mobInfoWindowView;
    }
}