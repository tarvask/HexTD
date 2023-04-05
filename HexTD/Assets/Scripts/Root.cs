using UnityEngine;

public class Root : MonoBehaviour
{
    private MainMenuController _mainMenuController;
        
    private void Start()
    {
        MainMenuView mainMenuView = FindObjectOfType<MainMenuView>();
        _mainMenuController = new MainMenuController(new MainMenuController.Context(mainMenuView));
    }
}