using Configs;
using Plugins.Zenject.Source.Install;
using UI.InfoPanel;
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
        
        public override void InstallBindings()
        {
            Container.Bind<UiPrefabHolder>().FromInstance(uiPrefabHolder).AsSingle();
            
            Container.Bind<PathsEditorInfoPanelView>().FromInstance(pathsEditorInfoPanelView).AsSingle();
            Container.Bind<PathsEditorInfoPanelController>().FromNew().AsSingle().NonLazy();

            Container.Bind<EditorInfoPanelView>().FromInstance(editorInfoPanelView).AsSingle();
            Container.Bind<EditorInfoPanelController>().FromNew().AsSingle().NonLazy();
        }
    }
}