using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tools
{
    public static class PointerOverUiElementDetector
    {
        private const int RaycastMaxDepth = 128;
        private static readonly List<RaycastResult> RaycastResults = new(RaycastMaxDepth);
        
        // Returns 'true' if we touched or hovering on Unity UI element.
        public static bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }
        
        // Returns 'true' if we touched or hovering on Unity UI element.
        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults )
        {
            int uiLayerIndex = LayerMask.NameToLayer("UI");
            
            foreach (var curRaycastResult in eventSystemRaycastResults)
            {
                if (curRaycastResult.gameObject.layer == uiLayerIndex)
                    return true;
            }

            return false;
        }
        
        // Gets all event system raycast results of current mouse or touch position.
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {   
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            RaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, RaycastResults);
            return RaycastResults;
        }
    }
}