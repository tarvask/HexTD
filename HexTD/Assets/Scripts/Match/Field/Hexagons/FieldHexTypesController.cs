using System.Collections.Generic;
using HexSystem;

namespace Match.Field.Hexagons
{
    public class FieldHexTypesController
    {
        private readonly IDictionary<int, FieldHexType> _currentFieldHexTypes;

        public FieldHexType this[int hashCode] => _currentFieldHexTypes[hashCode];
        public FieldHexType this[Hex2d position] => _currentFieldHexTypes[position.GetHashCode()];
        public int HexGridSize => _currentFieldHexTypes.Count;

        public FieldHexTypesController(IDictionary<int, FieldHex> fieldHexes)
        {
            _currentFieldHexTypes = new Dictionary<int, FieldHexType>(fieldHexes.Count);
            foreach (var fieldHex in fieldHexes)
            {
                _currentFieldHexTypes.Add(fieldHex.Key, fieldHex.Value.HexType);
            }
        }

        public bool TryAddTower(int key)
        {
            if (_currentFieldHexTypes[key] != FieldHexType.Free)
                return false;
            
            _currentFieldHexTypes[key] = FieldHexType.Tower;
            return true;
        }

        public bool TryRemoveTower(int key)
        {
            if (_currentFieldHexTypes[key] != FieldHexType.Tower)
                return false;
            
            _currentFieldHexTypes[key] = FieldHexType.Free;
            return true;
        }

        public void ForceSetHexType(int key, FieldHexType hexType)
        {
            _currentFieldHexTypes[key] = hexType;
        }

        public byte[] GetFieldHexTypes()
        {
            int size = _currentFieldHexTypes.Count;
            byte[] types = new byte[size];
            var hexEnumerator = _currentFieldHexTypes.Values.GetEnumerator();
            for(int i = 0; i < size && hexEnumerator.MoveNext(); i++)
            {
                types[i] = (byte)hexEnumerator.Current;
            }

            return types;
        }
    }
}