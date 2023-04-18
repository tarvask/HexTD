﻿using HexSystem;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public interface IHexPositionConversionService
    {
        Vector3 GetPlanePosition(Hex2d hexPosition);
        Vector3 GetWorldPosition(Hex3d hexPosition);
        Vector3 GetWorldPosition(Hex2d hexPosition);
        Vector3 GetUpHexWorldPosition(Hex2d hexPosition);
        Hex2d ToHexFromWorldPosition(Vector3 position);
        bool IsCloseToNewHex(float distanceToHex);
        Vector3 SingleHexSize { get; }
    }
}