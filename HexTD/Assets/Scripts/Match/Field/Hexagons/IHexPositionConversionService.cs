using HexSystem;
using UnityEngine;

namespace Match.Field.Hexagons
{
    public interface IHexPositionConversionService
    {
        Vector3 GetPlanePosition(Hex2d hexPosition, bool isWorld = true);
        Vector3 GetHexPosition(Hex3d hexPosition, bool isWorld = true);
        Vector3 GetHexPosition(Hex2d hexPosition, bool isWorld = true);
        Vector3 GetUpHexPosition(Hex2d hexPosition, bool isWorld = true);
        Hex2d ToHexFromWorldPosition(Vector3 position, bool isWorld = true);
        float GetRadiusFromRadiusInHex(int radius);
        bool IsCloseToNewHex(float distanceToHex);
    }
}