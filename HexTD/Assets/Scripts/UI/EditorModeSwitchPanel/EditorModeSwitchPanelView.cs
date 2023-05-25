using UnityEngine;
using UnityEngine.UI;

namespace UI.EditorModeSwitchPanel
{
    public class EditorModeSwitchPanelView : MonoBehaviour
    {
        [Header("Hex or Path modes toggle group")]
        [SerializeField] private ToggleGroup hexPathModesToggleGroup;
        
        [SerializeField] private Toggle hexModeToggle;
        [SerializeField] private Toggle pathModeToggle;

        [Header("Hex types toggle group")]
        [SerializeField] private ToggleGroup hexTypeModeToggleGroup;

        [SerializeField] private Toggle normalHexToggle;
        [SerializeField] private Toggle bridgeHexToggle;
        [SerializeField] private Toggle stonePropsHexToggle;
        [SerializeField] private Toggle bushPropsHexToggle;
        [SerializeField] private Toggle treePropsHexToggle;
        [SerializeField] private Toggle grassPropsHexToggle;
        [SerializeField] private Toggle mushroomSinglePropsHexToggle;
        [SerializeField] private Toggle mushroomClusterPropsHexToggle;

        public ToggleGroup HexPathModesToggleGroup => hexPathModesToggleGroup;
        public Toggle HexModeToggle => hexModeToggle;
        public Toggle PathModeToggle => pathModeToggle;
        
        public ToggleGroup HexTypeModeToggleGroup => hexTypeModeToggleGroup;
        public Toggle NormalHexToggle => normalHexToggle;
        public Toggle BridgeHexToggle => bridgeHexToggle;
        public Toggle StonePropsHexToggle => stonePropsHexToggle;
        public Toggle BushPropsHexToggle => bushPropsHexToggle;
        public Toggle TreePropsHexToggle => treePropsHexToggle;
        public Toggle GrassPropsHexToggle => grassPropsHexToggle;
        public Toggle MushroomSinglePropsHexToggle => mushroomSinglePropsHexToggle;
        public Toggle MushroomClusterPropsHexToggle => mushroomClusterPropsHexToggle;
    }
}