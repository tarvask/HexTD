using Configs;
using HexSystem;
using InputSystem;
using MapEditor;
using MapEditor.CustomHex;
using PathSystem;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MapEditorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EditorPointerInputHandler>().FromNew().AsSingle();
            Container.Bind<HexMapEditorController>().FromNew().AsSingle();
            Container.Bind<HeightHexSetController>().FromNew().AsSingle();
            Container.Bind<RotationHexSetController>().FromNew().AsSingle();
            Container.Bind<EditorPathContainer>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<PathEditorController>().FromNew().AsSingle();
            Container.Bind<InputRecipient>().FromNew().AsSingle();

            GameObject gameLoopObject = new GameObject("GameController");
            LevelMapEditor levelMapEditor = gameLoopObject.AddComponent<LevelMapEditor>();
            Container.QueueForInject(levelMapEditor);
        }
    }
}