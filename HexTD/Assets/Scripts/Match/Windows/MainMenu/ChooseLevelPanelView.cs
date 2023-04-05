using Tools;
using UnityEngine;

namespace Match.Windows.MainMenu
{
    public class ChooseLevelPanelView : BaseMonoBehaviour
    {
        [SerializeField] private Transform levelItemsRoot;
        [SerializeField] private ChooseLevelItemView levelItemPrefab;

        public Transform LevelItemsRoot => levelItemsRoot;
        public ChooseLevelItemView LevelItemPrefab => levelItemPrefab;
    }
}