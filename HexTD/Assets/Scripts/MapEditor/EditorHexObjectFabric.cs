using Configs;
using HexSystem;
using Match.Field;
using UnityEngine;

namespace MapEditor
{
    public class EditorHexObjectFabric : HexObjectFabric
    {
        private readonly Transform _hexsRoot;
        
        public EditorHexObjectFabric(HexagonPrefabConfig hexPrefabConfig) : base(hexPrefabConfig)
        {
            // create cells root
            _hexsRoot = new GameObject("Hexes").transform;
            _hexsRoot.SetAsLastSibling();
            _hexsRoot.localPosition = Vector3.zero;
            _hexsRoot.localScale = Vector3.one;
        }

        public HexObject Create(HexModel model, Vector3 position)
        {
            return Create(model, _hexsRoot, position);
        }
    }
}