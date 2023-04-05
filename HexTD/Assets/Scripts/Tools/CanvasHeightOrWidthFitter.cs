using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class CanvasHeightOrWidthFitter : MonoBehaviour
    {
        public void Process()
        {
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();

            if (canvasScaler == null)
                return;

            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null)
                return;

            float referenceResolutionAspectRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
            float displayResolutionAspectRatio = canvas.renderingDisplaySize.x / canvas.renderingDisplaySize.y;

            if (referenceResolutionAspectRatio >= displayResolutionAspectRatio)
                canvasScaler.matchWidthOrHeight = 0;
            else
                canvasScaler.matchWidthOrHeight = 1;
        }
    }
}