using System;
using Tools;
using UnityEngine;

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

        public GameObject GetGroundCell()
        {
            return _context.FieldConfig.GroundPrefab;
        }

        public GameObject GetCellByType(FieldCellType cellType)
        {
            switch (cellType)
            {
                case FieldCellType.Free:
                    return _context.FieldConfig.FreeCellPrefab;
                    
                case FieldCellType.Road:
                    return _context.FieldConfig.RoadCellPrefab;
                
                case FieldCellType.Unavailable:
                    return _context.FieldConfig.UnavailableCellPrefab;
                    
                case FieldCellType.Blocker:
                    return _context.FieldConfig.BlockerCellPrefab;
                    
                case FieldCellType.Tower:
                    return _context.FieldConfig.TowerCellPrefab;
                    
                case FieldCellType.Castle:
                    return _context.FieldConfig.CastleCellPrefab;
            }
            
            throw new ArgumentException("Unknown or undefined cell type");
        }
    }
}