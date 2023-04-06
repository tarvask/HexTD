using Tools;
using UnityEngine;

namespace Services
{
    public class MatchCanvasBuilder : BaseMonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        
        [Space]
        [Header("Fields")]
        [SerializeField] private RectTransform topField;
        [SerializeField] private RectTransform bottomField;

        [Space]
        [Header("Panels")]
        [SerializeField] private RectTransform middlePanel;
        [SerializeField] private RectTransform bottomPanel;
        
        [Header("Borders")]
        [SerializeField] private int topBorderThickness;
        [SerializeField] private int bottomBorderThickness;
        [SerializeField] private int leftAndRightBorderThickness;
        
        [Space]
        [Header("Panels")]
        [SerializeField] private int minBottomPanelHeight;

        public RectTransform OurField => bottomField;
        public RectTransform EnemyField => topField;
        public RectTransform MiddlePanel => middlePanel;

        public void Process()
        {
            int fieldWidthInCells = 10;
            int fieldHeightInCells = 10;

            // find right field size
            Rect screenRect = ((RectTransform) canvas.transform).rect;
            float screenHeight = screenRect.height;
            float halfScreenHeight = screenHeight * 0.5f;
            float screenWidth = screenRect.width;
            float maxFieldHeight = (screenHeight - topBorderThickness - bottomBorderThickness
                                    - middlePanel.rect.height - minBottomPanelHeight) * 0.5f;
            float maxFieldWidth = screenWidth - leftAndRightBorderThickness * 2;
            float widthForMaxFieldHeight = maxFieldHeight * fieldWidthInCells / fieldHeightInCells;
            float heightForMaxFieldWidth = maxFieldWidth * fieldHeightInCells / fieldWidthInCells;

            float realFieldWidth;
            float realFieldHeight;
            
            if (maxFieldWidth / maxFieldHeight < (float)fieldWidthInCells / fieldHeightInCells)
            {
                realFieldWidth = maxFieldWidth;
                realFieldHeight = heightForMaxFieldWidth;
            }
            else
            {
                realFieldWidth = widthForMaxFieldHeight;
                realFieldHeight = maxFieldHeight;
            }

            // find right fields' position
            float middlePanelPosX = 0;
            // middle panel y is median from top and bottom edges
            float middlePanelPosY = screenHeight - topBorderThickness
                                     - realFieldHeight - middlePanel.rect.height * 0.5f;
            float fieldPosX = 0;
            
            // top field y is clipped to middle panel from the top
            float topFieldPosY = middlePanelPosY + middlePanel.rect.height * 0.5f + realFieldHeight * 0.5f;
            // bottom field y is median from bottom edge and middle panel bottom
            float bottomFieldPosY = middlePanelPosY - middlePanel.rect.height * 0.5f - realFieldHeight * 0.5f;
            
            // find right bottom panel position
            // stretch it from screen bottom to bottom field edge
            float bottomPanelPosX = 0;
            float bottomPanelHeight = bottomFieldPosY - realFieldHeight * 0.5f - bottomBorderThickness;
            float bottomPanelPosY = 0;
            
            // compute with anchor and pivot
            middlePanelPosY = middlePanelPosY - halfScreenHeight;
            topFieldPosY = topFieldPosY - halfScreenHeight;
            bottomFieldPosY = bottomFieldPosY - halfScreenHeight;

            // place middle panel
            middlePanel.anchoredPosition = new Vector2(middlePanelPosX, middlePanelPosY);
            // place bottom panel
            bottomPanel.anchoredPosition = new Vector2(bottomPanelPosX, bottomPanelPosY);
            bottomPanel.sizeDelta = new Vector2(screenWidth, bottomPanelHeight);
            // place fields and set their size
            topField.anchoredPosition = new Vector2(fieldPosX, topFieldPosY);
            topField.sizeDelta = new Vector2(realFieldWidth, realFieldHeight);
            bottomField.anchoredPosition = new Vector2(fieldPosX, bottomFieldPosY);
            bottomField.sizeDelta = new Vector2(realFieldWidth, realFieldHeight);
        }
    }
}