using UnityEngine;

namespace Match.Field
{
    public class FieldMeasuresComputable //: BaseDisposable
    {
        public float TargetCellSizeInUnits { get; }
        public Vector2 BottomLeftCornerPosition { get; }
        public Vector2 TopRightCornerPosition { get; }

        public FieldMeasuresComputable(FieldConfig fieldConfig,
                                        int screenWidth,
                                        int screenHeight)
        {
//            Debug.Log($"Screen width: {screenWidth}, screen height: {screenHeight}");

//            int pureFieldWidth = screenWidth - fieldConfig.FieldSideBoundsThickness * 2;
//            int pureFieldHeight = //(int) (screenHeight * fieldConfig.PuzzlePartOnScreen)
//                screenHeight;
//                                  //- fieldConfig.FieldTopBoundThickness
//                                  //- fieldConfig.FieldBottomBoundThickness;
//
//            int maxCellWidth = pureFieldWidth / fieldConfig.FieldWidthInCells;
//            int maxCellHeight = pureFieldHeight / fieldConfig.FieldHeightInCells;
////            Debug.Log($"Max cell width: {maxCellWidth}, max cell height: {maxCellHeight}");
//            TargetCellSizeInPixels = Mathf.Min(maxCellWidth, maxCellHeight);
////            Debug.Log($"Target cell size: {TargetCellSizeInPixels}");
//
            // CellTexturePixelsPerUnit = (int) cellSprite.pixelsPerUnit;
//
//            // shift from left bound for a half cell size to get position of cell center, then convert pixels to units
//            float cellsStartXInPixels = -TargetCellSizeInPixels * fieldConfig.FieldWidthInCells * 0.5f
//                                        + TargetCellSizeInPixels * 0.5f;
//            // shift from bottom bound for a half cell size to get position of cell center, then convert pixels to units
//            float cellsStartYInPixels = -TargetCellSizeInPixels * fieldConfig.FieldHeightInCells * 0.5f
//                                        + TargetCellSizeInPixels * 0.5f;
//            CellsStartPositionInUnits = new Vector2(cellsStartXInPixels / CellTexturePixelsPerUnit,
//                cellsStartYInPixels / CellTexturePixelsPerUnit);
//
            // CellTextureSizeInUnits = (float) cellSprite.texture.width / CellTexturePixelsPerUnit;
//			
//            TargetCellSizeInUnits = (float) TargetCellSizeInPixels / CellTexturePixelsPerUnit;
            TargetCellSizeInUnits = 1.0f;
            
            //TargetCellScale = TargetCellSizeInUnits / CellTextureSizeInUnits;
            
            TopRightCornerPosition =
                new Vector2(TargetCellSizeInUnits * fieldConfig.FieldWidthInCells * 0.5f,
                    TargetCellSizeInUnits * fieldConfig.FieldHeightInCells * 0.5f);
            BottomLeftCornerPosition = -TopRightCornerPosition;
        }
    }
}