using Configs;
using Configs.Constants;
using HexSystem;
using MapEditor;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class HexGridInstaller : MonoInstaller
    {
        [SerializeField] private HexSettingsConfig hexSettingsConfig;
        [SerializeField] private HexagonPrefabConfig hexagonPrefabConfig;

        public override void InstallBindings()
        {
            Container.Bind<HexSettingsConfig>().FromInstance(hexSettingsConfig).AsSingle();
            Container.Bind<HexagonPrefabConfig>().FromInstance(hexagonPrefabConfig).AsSingle();

            var hexLayout = new Layout(hexSettingsConfig.HexSize, Vector3.zero,  hexSettingsConfig.IsFlat);
            Container.Bind<Layout>().FromInstance(hexLayout).AsSingle();
            Container.Bind<HexFabric>().FromNew().AsSingle();
            Container.Bind<HexGridModel>().FromNew().AsSingle();
            Container.Bind<HexSpawnerController>().FromNew().AsSingle();

            Container.Bind<HexObject>().FromInstance(hexagonPrefabConfig.BridgeHexObject).AsSingle();
            
            Container.Bind<HexInteractService>().FromNew().AsSingle();
            Container.Bind<LevelEditorSaveController>().FromNew().AsSingle();
        }
    }
}