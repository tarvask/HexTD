using UnityEngine;

namespace Services
{
    public static class FieldsAndCameraInstaller
    {
        private static readonly Vector3 FieldPivotShift = new Vector3(0.5f, 0.5f);
        
        public static void InstallFieldsByMarkerRects(Canvas canvas, Camera mainCamera,
            Transform ourFieldTransform, Transform enemyFieldTransform,
            SpriteRenderer enemyFieldBackground, SpriteRenderer ourFieldBackground,
            RectTransform ourFieldCanvasMarkerRect, RectTransform enemyFieldCanvasMarkerRect,
            RectTransform middlePanelRect)
        {
            //Rect screenRect = ((RectTransform) canvas.transform).rect;
            //Vector2 screenCenter = new Vector2(screenRect.width * 0.5f, screenRect.height * 0.5f);
            //float cameraPositionZ = -mainCamera.transform.localPosition.z;
            //
            //// convert 2 points of rectangle to world
            //// our field left bottom
            //Vector3 ourFieldLeftBottomCornerCanvasPosition =
            //    (screenCenter + ourFieldCanvasMarkerRect.anchoredPosition + ourFieldCanvasMarkerRect.rect.min);
            //// enemy field right top
            //Vector3 enemyFieldRightTopCornerCanvasPosition =
            //    (screenCenter + enemyFieldCanvasMarkerRect.anchoredPosition + enemyFieldCanvasMarkerRect.rect.max);
//
            //SetEnemyFieldPosition(canvas, ourFieldTransform, enemyFieldTransform,
            //    ourFieldCanvasMarkerRect, enemyFieldCanvasMarkerRect);
            //
            //// set camera strictly to fields' superposition
            //Vector3 fieldsSuperWorldPosition =
            //    (ourFieldTransform.localPosition - FieldPivotShift // real field rendering corner
            //        + enemyFieldTransform.localPosition + new Vector3(fieldWidthInCells, fieldHeightInCells) - FieldPivotShift) * 0.5f; // real field rendering corner
            //fieldsSuperWorldPosition = new Vector3(fieldsSuperWorldPosition.x, fieldsSuperWorldPosition.y, -cameraPositionZ);
            //mainCamera.transform.localPosition = fieldsSuperWorldPosition;
            //
            //// set enemy background to fields' superposition by its bottom edge
            //float enemyBackgroundPositionY = fieldsSuperWorldPosition.y + enemyFieldBackground.bounds.extents.y - enemyFieldTransform.localPosition.y;
            //enemyFieldBackground.transform.localPosition = new Vector3(enemyFieldBackground.transform.localPosition.x, enemyBackgroundPositionY);
            //
            //// set our background to fields' superposition by its top edge
            //float ourBackgroundPositionY = fieldsSuperWorldPosition.y - ourFieldBackground.bounds.extents.y - ourFieldTransform.localPosition.y;
            //ourFieldBackground.transform.localPosition = new Vector3(ourFieldBackground.transform.localPosition.x, ourBackgroundPositionY);
            //
            //SetCameraZoom(canvas, mainCamera,
            //    ourFieldLeftBottomCornerCanvasPosition, enemyFieldRightTopCornerCanvasPosition);
//
            //SetCameraPosition(canvas, mainCamera, middlePanelRect, ref fieldsSuperWorldPosition);
            //
            //// clean up
            //ourFieldCanvasMarkerRect.gameObject.SetActive(false);
            //enemyFieldCanvasMarkerRect.gameObject.SetActive(false);
        }

        private static void SetEnemyFieldPosition(Canvas canvas,
            Transform ourFieldTransform, Transform enemyFieldTransform,
            RectTransform ourFieldCanvasMarkerRect, RectTransform enemyFieldCanvasMarkerRect)
        {
            Rect screenRect = ((RectTransform) canvas.transform).rect;
            
            Vector2 screenCenter = new Vector2(screenRect.width * 0.5f, screenRect.height * 0.5f);
            // convert 2 points of rectangle to world
            // our field left bottom
            Vector3 ourFieldLeftBottomCornerCanvasPosition =
                (screenCenter + ourFieldCanvasMarkerRect.anchoredPosition + ourFieldCanvasMarkerRect.rect.min);
            //Vector3 ourFieldLeftBottomCornerScreenPosition = ourFieldLeftBottomCornerCanvasPosition * canvas.scaleFactor;
            //ourFieldLeftBottomCornerScreenPosition = new Vector3(ourFieldLeftBottomCornerScreenPosition.x, ourFieldLeftBottomCornerScreenPosition.y, cameraPositionZ);
            //Vector3 ourFieldLeftBottomCornerPosition = mainCamera.ScreenToWorldPoint(ourFieldLeftBottomCornerScreenPosition);
            
            // enemy field right top
            Vector3 enemyFieldRightTopCornerCanvasPosition =
                (screenCenter + enemyFieldCanvasMarkerRect.anchoredPosition + enemyFieldCanvasMarkerRect.rect.max);
            //Vector3 enemyFieldRightTopCornerScreenPosition = enemyFieldRightTopCornerCanvasPosition * canvas.scaleFactor;
            //enemyFieldRightTopCornerScreenPosition = new Vector3(enemyFieldRightTopCornerScreenPosition.x, enemyFieldRightTopCornerScreenPosition.y, cameraPositionZ);
            //Vector3 enemyFieldRightTopCornerPosition = mainCamera.ScreenToWorldPoint(enemyFieldRightTopCornerScreenPosition);

            // for orthographic camera
            //float cameraUnitSize = fieldTargetHeightInCameraView / fieldHeightInCells;;

            float fieldWidthToFieldsWithGapRatio = ourFieldCanvasMarkerRect.rect.width /
                                                   (enemyFieldRightTopCornerCanvasPosition.y -
                                                    ourFieldLeftBottomCornerCanvasPosition.y);

            // set enemy field position
            //float enemyFieldY = ourFieldTransform.localPosition.y + fieldWidthInCells / fieldWidthToFieldsWithGapRatio - fieldHeightInCells;
            //enemyFieldTransform.localPosition = new Vector3(ourFieldTransform.localPosition.x, enemyFieldY, 0);
        }

        private static void SetCameraZoom(Canvas canvas, Camera mainCamera,
            Vector2 ourFieldLeftBottomCornerCanvasPosition, Vector2 enemyFieldRightTopCornerCanvasPosition,
            int fieldWidthInCells)
        {
            Rect screenRect = ((RectTransform) canvas.transform).rect;
            // set camera distance
            //float currentFrustumHeightHalf = cameraPositionZ * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float fieldWidthToCanvasWidthRatio = (enemyFieldRightTopCornerCanvasPosition.x - ourFieldLeftBottomCornerCanvasPosition.x) / screenRect.width;
            float fieldWorldWidth = fieldWidthInCells;
            float newFrustumWidth = fieldWorldWidth / fieldWidthToCanvasWidthRatio;
            float newFrustumHeightHalf = 0.5f * newFrustumWidth * screenRect.height / screenRect.width;
            
            // newFrustumHeightHalf = newCameraPositionZ * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float newCameraPositionZ = newFrustumHeightHalf / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            mainCamera.transform.localPosition = new Vector3(
                mainCamera.transform.localPosition.x,
                mainCamera.transform.localPosition.y,
                -newCameraPositionZ);
        }

        private static void SetCameraPosition(Canvas canvas, Camera mainCamera, RectTransform middlePanelRect,
            ref Vector3 fieldsSuperWorldPosition)
        {
            Rect screenRect = ((RectTransform) canvas.transform).rect;
            Vector2 screenCenter = new Vector2(screenRect.width * 0.5f, screenRect.height * 0.5f);
            
            // find world position of middle panel center with new zoom
            Vector3 middlePanelCenterCanvasPosition = screenCenter + middlePanelRect.anchoredPosition;
            Vector3 middlePanelCenterScreenPosition = middlePanelCenterCanvasPosition * canvas.scaleFactor;
            middlePanelCenterScreenPosition = new Vector3(middlePanelCenterScreenPosition.x, middlePanelCenterScreenPosition.y, -mainCamera.transform.localPosition.z);
            Vector3 middlePanelCenterWorldPosition = mainCamera.ScreenToWorldPoint(middlePanelCenterScreenPosition);
            float cameraShiftY = middlePanelCenterWorldPosition.y - fieldsSuperWorldPosition.y;
            
            // shift vertically to correspond MatchInfo panel
            mainCamera.transform.localPosition -= new Vector3(0, cameraShiftY);
        }
    }
}