using Tools;
using UnityEngine;

namespace Match.Windows
{
    public class BaseWindowView : BaseMonoBehaviour
    {
        [SerializeField] private bool isBlockingClicksToField;

        public bool IsBlockingClicksToField => isBlockingClicksToField;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}