using System.Collections.Generic;
using Configs.Constants;
using HexSystem;
using UnityEngine;

namespace MapEditor
{
    public class HexSpawnerController
    {
        private readonly HexGridModel _hexGridModel;
        private string _currentHexTypeName;

        public HexSpawnerController(HexGridModel hexGridModel)
        {
            _hexGridModel = hexGridModel;
            _currentHexTypeName = HexTypeNameConstants.SimpleType;
        }

        public void CreateHex(Hex2d position)
        {
            List<(string, string)> parameters = new List<(string, string)>();
            parameters.Add((HexParamsNameConstants.HexTypeParam, _currentHexTypeName));
            parameters.Add((HexParamsNameConstants.HexRotationParam, "0"));
            HexModel hexModel = _hexGridModel.CreateHex(position, parameters);
        }

        public void UpdateHexType()
        {
            _currentHexTypeName = GetInputtedHexType();
        }

        public void SetHexType(string hexType)
        {
            _currentHexTypeName = hexType;
        }

        private string GetInputtedHexType()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                return HexTypeNameConstants.SimpleType;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                return HexTypeNameConstants.BridgeType;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                return HexTypeNameConstants.StonePropsType;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                return HexTypeNameConstants.BushPropsType;
            if (Input.GetKeyDown(KeyCode.Alpha5))
                return HexTypeNameConstants.TreePropsType;
            if (Input.GetKeyDown(KeyCode.Alpha6))
                return HexTypeNameConstants.GrassPropsType;

            return _currentHexTypeName;
        }
    }
}