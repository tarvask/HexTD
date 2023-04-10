﻿using System.Collections.Generic;
using HexSystem;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public class HexagonalFieldModel : IHexPositionConversationService
    {
        private readonly Layout _layout;
        private readonly IDictionary<int, FieldHex> _cachedLevelFieldHexes;
        
       public FieldHexTypesController CurrentOurFieldHexes { get; }
       public FieldHexTypesController CurrentEnemyFieldHexes { get; }

        public int HexGridSize => _cachedLevelFieldHexes.Count;

        public HexModel this[int positionHash] => _cachedLevelFieldHexes[positionHash].HexModel;
        public bool IsHexInMap(int positionHash) => _cachedLevelFieldHexes.ContainsKey(positionHash);        
        
        public FieldHexType GetOurHexTypeByPosition(Hex2d position) => CurrentOurFieldHexes[position.GetHashCode()];
        public FieldHexType GetEnemyHexTypeByPosition(Hex2d position) => CurrentEnemyFieldHexes[position.GetHashCode()];

        public HexagonalFieldModel(Layout layout, FieldHex[] fieldHexes)
        {
            _layout = layout;
            
            _cachedLevelFieldHexes = new Dictionary<int, FieldHex>();

            foreach (var fieldHex in fieldHexes)
            {
                FieldHex cachedFieldHex = new FieldHex(fieldHex.HexModel, fieldHex.HexType);
                _cachedLevelFieldHexes.Add(fieldHex.HexModel.GetHashCode(), cachedFieldHex);
            }
            
            CurrentOurFieldHexes = new FieldHexTypesController(_cachedLevelFieldHexes);
            CurrentEnemyFieldHexes = new FieldHexTypesController(_cachedLevelFieldHexes);
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

        public void Reset()
        {
            foreach (var fieldHexHashPair in _cachedLevelFieldHexes)
            {
                CurrentOurFieldHexes.ForceSetHexType(fieldHexHashPair.Key, fieldHexHashPair.Value.HexType);
            }
        }
    }
}