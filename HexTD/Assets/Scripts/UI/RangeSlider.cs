using UnityEngine;

public class RangeSlider : MonoBehaviour
{
    public void CalculateSliderPosition(RectTransform slider, int minValue, int maxValue)
    {
        var spriteCenter = (maxValue + minValue) / 2;
        var spriteLenght = maxValue - minValue;

        var anchorMin = (spriteCenter - (spriteLenght / 2)) / 100f;
        var anchorMax = (spriteCenter + (spriteLenght / 2)) / 100f;

        slider.anchorMin = new Vector2(anchorMin, 0f);
        slider.anchorMax = new Vector2(anchorMax, 1f);
        slider.pivot = new Vector2(0.5f, 0.5f);
    }
}