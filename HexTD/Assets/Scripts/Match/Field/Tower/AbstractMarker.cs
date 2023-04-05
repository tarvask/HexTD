using UnityEditor;
using UnityEngine;

namespace Match.Field.Tower
{
    public abstract class AbstractMarker : MonoBehaviour
    {
        protected abstract int NumberOfFields { get; }
        
#if UNITY_EDITOR
        public const int ReferenceHeight = 20;
        protected const int PropertyHeight = 20;
        private const float PropertyTitleWidthPart = 0.35f;
        
        public int GetHeightInInspector()
        {
            return PropertyHeight * NumberOfFields;
        }

        public virtual void DrawInInspector(Rect position, SerializedProperty property)
        {
            // reference
            Rect referenceRect = position;
            referenceRect.height = ReferenceHeight;
            property.objectReferenceValue = EditorGUI.ObjectField(referenceRect,
                property.objectReferenceValue,
                typeof(AbstractAbilityMarker), false);
        }

        protected void DrawProperty(Rect position, string propertyName, string propertyValue)
        {
            Rect abilityNameTitleRect = position;
            abilityNameTitleRect.width = position.width * PropertyTitleWidthPart;
            GUI.Label(abilityNameTitleRect, propertyName);
            Rect abilityNameValueRect = position;
            abilityNameValueRect.width = position.width * (1 - PropertyTitleWidthPart);
            abilityNameValueRect.x = position.width * PropertyTitleWidthPart;
            GUI.Label(abilityNameValueRect, propertyValue);
        }
#endif
    }
}