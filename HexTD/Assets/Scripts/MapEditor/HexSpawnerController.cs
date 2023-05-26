using System.Collections.Generic;
using Configs.Constants;
using HexSystem;
using UnityEngine;

namespace MapEditor
{
    public class HexSpawnerController
    {
        private const string InvisibleHexTypeName = "invisible";
        
        private readonly EditorHexesModel _editorHexesModel;
        private string _currentHexTypeName;

        public HexSpawnerController(EditorHexesModel editorHexesModel)
        {
            _editorHexesModel = editorHexesModel;
            _currentHexTypeName = HexTypeNameConstants.SimpleType;
        }

        public void CreateHex(Hex2d position) => CreateHex(position, _currentHexTypeName);
        
        public void CreateInvisibleHex(Hex2d position) => CreateHex(position, InvisibleHexTypeName);
        
        public void CreateHex(Hex2d position, string hexTypeName)
        {
            List<(string, string)> parameters = new List<(string, string)>()
            {
                (HexParamsNameConstants.HexTypeParam, hexTypeName),
                (HexParamsNameConstants.HexRotationParam, "0"),
            };

            HexModel hexModel = _editorHexesModel.CreateHex(position, parameters);
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
                return HexTypeNameConstants.GrassType;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                return HexTypeNameConstants.BridgeType;

            return _currentHexTypeName;
        }
    }
}