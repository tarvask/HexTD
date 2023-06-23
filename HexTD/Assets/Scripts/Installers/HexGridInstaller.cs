using Configs;
using HexSystem;
using MapEditor;
using Match.Field;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class HexGridInstaller : MonoInstaller
    {
        [SerializeField] private HexSettingsConfig hexSettingsConfig;
        [SerializeField] private HexagonPrefabConfig hexagonPrefabConfig;
        [SerializeField] private PropsPrefabConfig propsPrefabConfig;

        public override void InstallBindings()
        {
            Container.Bind<HexSettingsConfig>().FromInstance(hexSettingsConfig).AsSingle();
            Container.Bind<HexagonPrefabConfig>().FromInstance(hexagonPrefabConfig).AsSingle();
            Container.BindInterfacesTo<HexPrefabConfigRetriever>().FromNew().AsSingle();
            Container.Bind<PropsPrefabConfig>().FromInstance(propsPrefabConfig).AsSingle();
            Container.BindInterfacesTo<PropsObjectPrefabConfigRetriever>().FromNew().AsSingle();

            var hexLayout = new Layout(hexSettingsConfig.HexSize, Vector3.zero,  hexSettingsConfig.IsFlat);
            Container.Bind<Layout>().FromInstance(hexLayout).AsSingle();
            
            Container.Bind<EditorHexObjectFabric>().FromNew().AsSingle();
            Container.Bind<EditorHexesModel>().FromNew().AsSingle();
            Container.Bind<HexSpawnerController>().FromNew().AsSingle();
            
            Container.Bind<EditorPropsObjectFabric>().FromNew().AsSingle();
            Container.Bind<EditorPropsModel>().FromNew().AsSingle();
            Container.Bind<PropsSpawnerController>().FromNew().AsSingle();

            Container.Bind<EditorHexInteractService>().FromNew().AsSingle();
            
            Container.Bind<LevelEditorSaveController>().FromNew().AsSingle();
        }
    }
}