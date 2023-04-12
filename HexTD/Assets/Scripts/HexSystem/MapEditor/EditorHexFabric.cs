using Configs;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class EditorHexFabric : HexFabric
    {
        private readonly Transform _hexsRoot;
        
        public EditorHexFabric(HexagonPrefabConfig hexagonPrefabConfig) : base(hexagonPrefabConfig)
        {
            // create cells root
            _hexsRoot = new GameObject("Cells").transform;
            _hexsRoot.SetAsLastSibling();
            _hexsRoot.localPosition = Vector3.zero;
            _hexsRoot.localScale = Vector3.one;
        }

        public HexObject CreateHexObject(HexModel hexModel, Vector3 position)
        {
            return CreateHexObject(hexModel, _hexsRoot, position);
        }
    }
}