using MapEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [CreateAssetMenu(fileName = "HexagonPrefabConfig", menuName = "Configs/HexagonPrefabConfig", order = 4)]
    public class HexagonPrefabConfig : ScriptableObject
    {
        [FormerlySerializedAs("simpleHexagon")] [SerializeField] private HexObject simpleHexObject;
        [FormerlySerializedAs("bridgeHexagon")] [SerializeField] private HexObject bridgeHexObject;

        public HexObject SimpleHexObject => simpleHexObject;
        public HexObject BridgeHexObject => bridgeHexObject;
    }
}