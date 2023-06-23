using UI.Tools.SimpleToggle;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

namespace UI.EditorModeSwitchPanel
{
    public class EditorModeSwitchPanelView : MonoBehaviour
    {
        [Header("Hex or Path modes toggle group")]
        [SerializeField] private ToggleGroup hexPathModesToggleGroup;
        
        [SerializeField] private Toggle hexModeToggle;
        [SerializeField] private Toggle propsModeToggle;
        [SerializeField] private Toggle pathModeToggle;

        [SerializeField] private SimpleToggleGroup hexTypeToggleGroup;
        [SerializeField] private ScrollRect hexTypeScrollRect;
        
        [SerializeField] private SimpleToggleGroup propsTypeToggleGroup;
        [SerializeField] private ScrollRect propsTypeScrollRect;

        public ToggleGroup HexPathModesToggleGroup => hexPathModesToggleGroup;
        public Toggle HexModeToggle => hexModeToggle;
        public Toggle PropsModeToggle => propsModeToggle;
        public Toggle PathModeToggle => pathModeToggle;
        
        public SimpleToggleGroup HexTypeToggleGroup => hexTypeToggleGroup;
        public ScrollRect HexTypeScrollRect => hexTypeScrollRect;
        public SimpleToggleGroup PropsTypeToggleGroup => propsTypeToggleGroup;
        public ScrollRect PropsTypeScrollRect => propsTypeScrollRect;
    }
}