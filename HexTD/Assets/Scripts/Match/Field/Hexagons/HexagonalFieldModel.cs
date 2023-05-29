using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Configs.Constants;
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
        public Vector3 HexSize => _layout.HexSize;

        public HexModel this[int positionHash] => _cachedLevelFieldHexes.ContainsKey(positionHash)
            ? _cachedLevelFieldHexes[positionHash].HexModel
            : null;
        
        public bool IsHexInMap(int positionHash) => _cachedLevelFieldHexes.ContainsKey(positionHash);        
        
        public FieldHexType GetHexTypeByPosition(Hex2d position) => CurrentFieldHexTypes[position.GetHashCode()];

        public HexagonalFieldModel(HexSettingsConfig hexSettingsConfig, Vector3 rootPosition, List<FieldHex> fieldHexes)
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

        public Vector3 GetPlanePosition(Hex2d hexPosition, bool isWorld = true)
        {
            return _layout.ToPlane(hexPosition, isWorld);
        }

        public Vector3 GetHexPosition(Hex3d hexPosition, bool isWorld = true)
        {
            return _layout.ToPlane(hexPosition, isWorld);
        }

        public Vector3 GetHexPosition(Hex2d hexPosition, bool isWorld = true)
        {
            FieldHex hex = _cachedLevelFieldHexes[hexPosition.GetHashCode()];
            return _layout.ToPlane(hex.HexModel.Q, hex.HexModel.R, hex.HexModel.Height, isWorld);
        }

        public Vector3 GetBottomHexPosition(Hex2d hexPosition, bool isWorld = true)
        {
            FieldHex hex = _cachedLevelFieldHexes[hexPosition.GetHashCode()];
            return _layout.ToPlane(hex.HexModel.Q, hex.HexModel.R, (int)(hex.HexModel.Height - _layout.HexSize.y), isWorld);
        }

        public Hex2d ToHexFromWorldPosition(Vector3 position, bool isWorld = true)
        {
            return (Hex2d)_layout.ToHex(position, isWorld).RoundToHex();
        }

        public float GetRadiusFromRadiusInHex(int radius)
        {
            return radius * _layout.HexSize.x;
        }

        public bool IsCloseToNewHex(float distanceToHex)
        {
            return distanceToHex < _layout.HexSize.y && distanceToHex < _layout.HexSize.x;
        }

        public bool IsHexInMap(Hex2d position)
        {
            return _cachedLevelFieldHexes.ContainsKey(position.GetHashCode());
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

        public bool IsOnSegment(Hex2d hex, Vector2 a, Vector2 b) => _layout.IsOnSegment(hex, a, b);

        public Bounds GetBounds()
        {
            Vector3 min, max;
            min = max = GetPlanePosition(_cachedLevelFieldHexes.Values.First().HexModel.Position);
            min.y = max.y = _cachedLevelFieldHexes.Values.First().HexModel.Height;

            foreach (var fieldHex in _cachedLevelFieldHexes.Values.Skip(1))
            {
                var nextPlanePosition = GetPlanePosition(fieldHex.HexModel.Position);
                var nextHeight = fieldHex.HexModel.Height;

                if (nextPlanePosition.x < min.x)
                {
                    min.x = nextPlanePosition.x;
                }

                if (nextPlanePosition.x > max.x)
                {
                    max.x = nextPlanePosition.x;
                }

                if (nextPlanePosition.z < min.z)
                {
                    min.z = nextPlanePosition.z;
                }

                if (nextPlanePosition.z > max.z)
                {
                    max.z = nextPlanePosition.z;
                }

                if (nextHeight < min.y)
                {
                    min.y = nextHeight;
                }

                if (nextHeight > max.y)
                {
                    max.y = nextHeight;
                }
            }

            var size = max - min;
            size = new Vector3(size.x, size.y * _layout.HexSize.y, size.z);
            
            Bounds bounds = new Bounds(min + size / 2.0f, size + _layout.HexSize);

            return bounds;
        }

        public bool GetHexIsBlocker(Hex2d hex)
        {
            return IsHexWithProperty(hex, HexParamsNameConstants.IsBlockerParam);
        }

        public bool GetHexIsRangeAttackBlocker(Hex2d hex)
        {
            return IsHexWithProperty(hex, HexParamsNameConstants.IsRangeAttackBlockerParam);
        }

        private bool IsHexWithProperty(Hex2d hex, string propertyName)
        {
            if (_cachedLevelFieldHexes.ContainsKey(hex.GetHashCode()))
            {
                if (_cachedLevelFieldHexes[hex.GetHashCode()].HexModel.Data.TryGetValue(
                        propertyName, out var propertyValue))
                {
                    return bool.Parse(propertyValue);
                }

                return false;
            }

            return true;
        }
    }
}