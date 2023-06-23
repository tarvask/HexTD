using HexSystem;
using InputSystem;
using MapEditor;
using MapEditor.CustomHex;
using MapEditor.CustomProps;
using PathSystem;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MapEditorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
//            Hex2d editableAreaSize = new Hex2d(14, 10);
            Hex2d editableAreaSize = new Hex2d(8, 15);
            
            Container.Bind<EditorPointerInputHandler>().FromNew().AsSingle();
            
            Container.BindInstance(editableAreaSize).WithId(EditorHexesController.KeyEditableAreaSize).AsSingle();
            
            Container.Bind<EditorHexesController>().FromNew().AsSingle();
            Container.Bind<HeightHexSetController>().FromNew().AsSingle();
            Container.Bind<RotationHexSetController>().FromNew().AsSingle();
            
            Container.Bind<EditorPropsController>().FromNew().AsSingle();
            Container.Bind<HeightPropsSetController>().FromNew().AsSingle();
            Container.Bind<RotationPropsSetController>().FromNew().AsSingle();
            
            Container.Bind<EditorPathContainer>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<PathEditorController>().FromNew().AsSingle();
            
            Container.Bind<InputRecipient>().FromNew().AsSingle();

            GameObject gameLoopObject = new GameObject("GameController");
            LevelMapEditor levelMapEditor = gameLoopObject.AddComponent<LevelMapEditor>();
            Container.QueueForInject(levelMapEditor);
        }
    }
}