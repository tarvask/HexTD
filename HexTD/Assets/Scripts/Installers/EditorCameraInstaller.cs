using Configs;
using InputSystem;
using Zenject;
using UnityEngine;

namespace Installers
{
    public class EditorCameraInstaller : MonoInstaller
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private CameraSettingsConfig cameraSettingsConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(mainCamera).AsSingle();
            Container.Bind<CameraSettingsConfig>().FromInstance(cameraSettingsConfig).AsSingle();
            Container.Bind<EditorCameraMovementService>().FromNew().AsSingle();
        }
    }
}