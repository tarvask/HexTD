using MapEditor;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "HexagonPrefabConfig", menuName = "Configs/HexagonPrefabConfig", order = 4)]
    public class HexagonPrefabConfig : ScriptableObject
    {
        [SerializeField] private HexObject simpleHexObject;
        [SerializeField] private HexObject bridgeHexObject;
        // props
        [SerializeField] private HexObject stonePropsHexObject;
        [SerializeField] private HexObject bushPropsHexObject;
        [SerializeField] private HexObject treePropsHexObject;
        [SerializeField] private HexObject grassPropsHexObject;
        [SerializeField] private HexObject mushroomSinglePropsHexObject;
        [SerializeField] private HexObject mushroomClusterPropsHexObject;

        public HexObject SimpleHexObject => simpleHexObject;
        public HexObject BridgeHexObject => bridgeHexObject;
        // props
        public HexObject StonePropsHexObject => stonePropsHexObject;
        public HexObject BushPropsHexObject => bushPropsHexObject;
        public HexObject TreePropsHexObject => treePropsHexObject;
        public HexObject GrassPropsHexObject => grassPropsHexObject;
        public HexObject MushroomSinglePropsHexObject => mushroomSinglePropsHexObject;
        public HexObject MushroomClusterPropsHexObject => mushroomClusterPropsHexObject;
    }
}