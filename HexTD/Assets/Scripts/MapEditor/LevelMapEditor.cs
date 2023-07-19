using InputSystem;
using UnityEngine;
using Zenject;

namespace MapEditor
{
    public class LevelMapEditor : MonoBehaviour
    {
        private InputRecipient _inputRecipient;
        private PathEditorController _pathEditorController;

        [Inject]
        public void Construct(InputRecipient inputRecipient,
            PathEditorController pathEditorController)
        {
            _inputRecipient = inputRecipient;
            _pathEditorController = pathEditorController;
        }
        
        private void Update()
        {
            _inputRecipient.ApplyInput(Time.deltaTime);
            _pathEditorController.DrawPath();
        }
    }
}