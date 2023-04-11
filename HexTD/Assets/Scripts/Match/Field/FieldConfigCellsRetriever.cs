using System;
using Configs.Constants;
using MapEditor;
using Tools;

namespace Match.Field
{
    public class FieldConfigCellsRetriever : BaseDisposable
    {
        public struct Context
        {
            public FieldConfig FieldConfig { get; }

            public Context(FieldConfig fieldConfig)
            {
                FieldConfig = fieldConfig;
            }
        }

        private readonly Context _context;
        
        public FieldConfigCellsRetriever(Context context)
        {
            _context = context;
        }

        public HexObject GetCellByType(string hexTypeName)
        {
            switch (hexTypeName)
            {
                case HexTypeNameConstants.SimpleType:
                    return _context.FieldConfig.HexagonPrefabConfig.SimpleHexObject;
                    
                case HexTypeNameConstants.BridgeType:
                    return _context.FieldConfig.HexagonPrefabConfig.BridgeHexObject;
            }
            
            throw new ArgumentException("Unknown or undefined cell type");
        }
    }
}