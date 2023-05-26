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

        //#85925650 дублирование
        public HexObject GetCellByType(string hexTypeName)
        {
            if (!_context.FieldConfig.HexagonPrefabConfig.HexObjects.TryGetValue(hexTypeName, out var hexObject))
            {
                throw new ArgumentException("Unknown or undefined cell type");
            }
            
            return hexObject;
        }
    }
}