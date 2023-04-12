using System.Collections;
using System.Collections.Generic;
using Configs;
using HexSystem;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public class HexagonalFieldModel : IHexPositionConversionService, IEnumerable<KeyValuePair<int, FieldHex>>
    {
        private readonly Layout _layout;
        private readonly IDictionary<int, FieldHex> _cachedLevelFieldHexes;
        
       public FieldHexTypesController CurrentFieldHexTypes { get; }

        public int HexGridSize => _cachedLevelFieldHexes.Count;

        public HexModel this[int positionHash] => _cachedLevelFieldHexes[positionHash].HexModel;
        public bool IsHexInMap(int positionHash) => _cachedLevelFieldHexes.ContainsKey(positionHash);        
        
        public FieldHexType GetHexTypeByPosition(Hex2d position) => CurrentFieldHexTypes[position.GetHashCode()];

        public HexagonalFieldModel(HexSettingsConfig hexSettingsConfig, Vector3 rootPosition, FieldHex[] fieldHexes)
        {
            _layout = new Layout(hexSettingsConfig.HexSize, rootPosition, hexSettingsConfig.IsFlat);
            _cachedLevelFieldHexes = new Dictionary<int, FieldHex>();

            foreach (var fieldHex in fieldHexes)
            {
                FieldHex cachedFieldHex = new FieldHex(fieldHex.HexModel, fieldHex.HexType);
                int hashKey = fieldHex.HexModel.GetHashCode();
                _cachedLevelFieldHexes.Add(hashKey, cachedFieldHex);
            }
            
            CurrentFieldHexTypes = new FieldHexTypesController(_cachedLevelFieldHexes);
        }

        public Vector3 GetPlanePosition(Hex2d hexPosition) => _layout.ToPlane(hexPosition);
        public Vector3 GetWorldPosition(Hex3d hexPosition) => _layout.ToPlane(hexPosition);
        
        public Vector3 GetWorldPosition(Hex2d hexPosition)
        {
            FieldHex hex = _cachedLevelFieldHexes[hexPosition.GetHashCode()];
            return _layout.ToPlane(hex.HexModel.Q, hex.HexModel.R, hex.HexModel.Height);
        }
        
        public Vector3 GetUpHexWorldPosition(Hex2d hexPosition)
        {
            FieldHex hex = _cachedLevelFieldHexes[hexPosition.GetHashCode()];
            return _layout.ToPlane(hex.HexModel.Q, hex.HexModel.R, hex.HexModel.Height + 1);
        }

        public Hex2d ToHexFromWorldPosition(Vector3 position)
        {
            return (Hex2d)_layout.ToHex(position).RoundToHex();
        }

        public bool IsCloseToNewHex(float distanceToHex)
        {
            return distanceToHex < _layout.Size.y && distanceToHex < _layout.Size.x;
        }

        public void Reset()
        {
            foreach (var fieldHexHashPair in _cachedLevelFieldHexes)
            {
                CurrentFieldHexTypes.ForceSetHexType(fieldHexHashPair.Key, fieldHexHashPair.Value.HexType);
            }
        }

        public IEnumerator<KeyValuePair<int, FieldHex>> GetEnumerator()
        {
            return _cachedLevelFieldHexes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}