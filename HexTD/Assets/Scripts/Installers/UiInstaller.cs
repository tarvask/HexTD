using Configs;
using UI.EditorInfoPanel;
using UI.EditorModeSwitchPanel;
using UI.PathEditorPanel;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class UiInstaller : MonoInstaller
    {
        [SerializeField] private UiPrefabHolder uiPrefabHolder;
        [Space]
        [SerializeField] private PathsEditorInfoPanelView pathsEditorInfoPanelView;
        [SerializeField] private EditorInfoPanelView editorInfoPanelView;
        [SerializeField] private EditorModeSwitchPanelView editorModeSwitchPanelView;
        
        public override void InstallBindings()
        {
            Container.Bind<UiPrefabHolder>().FromInstance(uiPrefabHolder).AsSingle();
            
            Container.Bind<PathsEditorInfoPanelView>().FromInstance(pathsEditorInfoPanelView).AsSingle();
            Container.Bind<PathsEditorInfoPanelController>().FromNew().AsSingle().NonLazy();

            Container.Bind<EditorInfoPanelView>().FromInstance(editorInfoPanelView).AsSingle();
            Container.Bind<EditorInfoPanelController>().FromNew().AsSingle().NonLazy();

            Container.Bind<EditorModeSwitchPanelView>().FromInstance(editorModeSwitchPanelView).AsSingle();
            Container.Bind<EditorModeSwitchPanelController>().FromNew().AsSingle().NonLazy();
        }
    }
}