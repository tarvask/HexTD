using UI.InfoPanel;
using UI.PathEditorPanel;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UiPrefabHolder", menuName = "Configs/UiPrefabHolder", order = 3)]
    public class UiPrefabHolder : ScriptableObject
    {
        [SerializeField] private PathsEditorInfoPanelView pathsEditorInfoPanelView;
        [SerializeField] private EditorInfoPanelView editorInfoPanelView;
        
        public PathsEditorInfoPanelView PathsEditorInfoPanelView => pathsEditorInfoPanelView;
        public EditorInfoPanelView EditorInfoPanelView => editorInfoPanelView;
    }
}