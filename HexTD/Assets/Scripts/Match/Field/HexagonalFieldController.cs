using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagonal;
using UnityEngine;

namespace Match.Field
{
    public class HexagonalFieldController
    {
        private readonly Layout _layout;
        private readonly IDictionary<int, FieldHex> _cachedLevelFieldHexes;
        private readonly IDictionary<int, FieldHex> _currentFieldHexes;

        public int HexGridSize => _currentFieldHexes.Count;

        public FieldHex this[Hex2d position] => _currentFieldHexes[position.GetHashCode()];

        public HexagonalFieldController(Layout layout, FieldHex[] fieldHexes)
        {
            _layout = layout;
            
            _cachedLevelFieldHexes = new Dictionary<int, FieldHex>();
            _currentFieldHexes = new Dictionary<int, FieldHex>();
            foreach (var fieldHex in fieldHexes)
            {
                _cachedLevelFieldHexes.Add(fieldHex.GetHashCode(), fieldHex);
                _currentFieldHexes.Add(fieldHex.GetHashCode(), fieldHex);
            }
        }

        public Vector3 GetPlanePosition(Hex2d hexPosition) => _layout.ToPlane(hexPosition);
        public Vector3 GetWorldPosition(Hex3d hexPosition) => _layout.ToPlane(hexPosition);
        public Vector3 GetWorldPosition(Hex2d hexPosition)
        {
            FieldHex hex = _currentFieldHexes[hexPosition.GetHashCode()];
            return _layout.ToPlane(hex.Q, hex.R, hex.H);
        }
        
        public bool TryAddTower(Hex2d position)
        {
            int hashCode = position.GetHashCode();
            if (!_currentFieldHexes.TryGetValue(hashCode, out var fieldHex))
                return false;

            return fieldHex.TryAddTower();
        }

        public bool TryRemoveTower(int positionHash)
        {
            if (!_currentFieldHexes.TryGetValue(positionHash, out var fieldHex))
                return false;

            return fieldHex.TryRemoveTower();
        }

        public byte[] GetFieldHexTypes()
        {
            int size = _currentFieldHexes.Count;
            byte[] types = new byte[size];
            var hexEnumerator = _currentFieldHexes.Values.GetEnumerator();
            for(int i = 0; i < size && hexEnumerator.MoveNext(); i++)
            {
                types[i] = (byte)hexEnumerator.Current.FieldHexType;
            }

            return types;
        }

        public void Reset()
        {
            _currentFieldHexes.Clear();
            foreach (var fieldHexHashPair in _cachedLevelFieldHexes)
            {
                _currentFieldHexes.Add(fieldHexHashPair.Key, fieldHexHashPair.Value);
            }
        }
    }
}