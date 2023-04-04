using MapEditor;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "HexSettingsConfig", menuName = "Configs/HexSettingsConfig", order = 2)]
    public class HexSettingsConfig : ScriptableObject
    {
        [SerializeField] private Vector3 hexSize;
        [SerializeField] private bool isFlat;

        public Vector3 HexSize => hexSize;
        public bool IsFlat => isFlat;
    }
}