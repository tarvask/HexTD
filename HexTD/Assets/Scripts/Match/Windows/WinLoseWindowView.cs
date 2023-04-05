using UnityEngine;
using UnityEngine.UI;

namespace Match.Windows
{
    public class WinLoseWindowView : BaseWindowView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject winGroup;
        [SerializeField] private GameObject loseGroup;

        public Button CloseButton => closeButton;
        public GameObject WinGroup => winGroup;
        public GameObject LoseGroup => loseGroup;
    }
}