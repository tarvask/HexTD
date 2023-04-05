using System;
using UnityEditor;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public class TowerLevelAbilitiesParams
    {
        [SerializeField] private AbstractAbilityMarker[] abilities;

        public AbstractAbilityMarker[] Abilities
        {
            get => abilities;
#if UNITY_EDITOR
            set => abilities = value;
#endif
        }

        public static class FieldNames
        {
            public static string Abilities => nameof(abilities);
        }
        
#if UNITY_EDITOR
        public const int AbilitiesLabelHeight = 15;
        
        public float GetHeightInInspector()
        {
            //if (_propertyHeight >= 0)
            //    return _propertyHeight;
             
            float propertyHeight = AbilitiesLabelHeight;

            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                AbstractAbilityMarker abilityProperty = abilities[abilityIndex];
                propertyHeight += AbstractMarker.ReferenceHeight +
                                   (abilityProperty?.GetHeightInInspector() ?? 0);
            }

            return propertyHeight;
        }

        public static int GetPropertyHeight(SerializedProperty property)
        {
            int propertyHeight = AbilitiesLabelHeight;
            SerializedProperty abilitiesArrayProperty = property.FindPropertyRelative(FieldNames.Abilities);

            for (int abilityIndex = 0; abilityIndex < abilitiesArrayProperty.arraySize; abilityIndex++)
            {
                SerializedProperty abilityProperty = abilitiesArrayProperty.GetArrayElementAtIndex(abilityIndex);
                propertyHeight += AbstractMarker.ReferenceHeight +
                                   (((AbstractAbilityMarker) abilityProperty.objectReferenceValue)?.GetHeightInInspector() ?? 0);
            }

            return propertyHeight;
        }
        
        public static void DrawInInspector(Rect position, SerializedProperty property, bool drawPropertyName)
        {
            Rect abilitiesLabelRect = position;
            abilitiesLabelRect.height = AbilitiesLabelHeight;
            
            if (drawPropertyName)
                GUI.Label(abilitiesLabelRect, "Abilities");
            
            float currentDelta = position.position.y + AbilitiesLabelHeight;
             
            SerializedProperty abilitiesArray = property.FindPropertyRelative(FieldNames.Abilities);

            for (int abilityIndex = 0; abilityIndex < abilitiesArray.arraySize; abilityIndex++)
            {
                SerializedProperty abilityProperty = abilitiesArray.GetArrayElementAtIndex(abilityIndex);
                AbstractAbilityMarker abilityMarkerValue = ((AbstractAbilityMarker) abilityProperty.objectReferenceValue);
                 
                Rect abilityGroupRect = position;
                abilityGroupRect.height = AbstractMarker.ReferenceHeight + (abilityMarkerValue?.GetHeightInInspector() ?? 0);
                abilityGroupRect.y = currentDelta;
                 
                GUI.BeginGroup(abilityGroupRect, new GUIStyle("box"));
                {
                    Rect abilityRect = abilityGroupRect;
                    abilityRect.x = 0;
                    abilityRect.y = 0;
                    abilityMarkerValue?.DrawInInspector(abilityRect, abilityProperty);

                    currentDelta += abilityGroupRect.height;
                }
                GUI.EndGroup();
            }
        }
#endif
    }
}