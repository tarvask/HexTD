using System;
using Match.Field.Shooting;
using UnityEditor;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public struct TowerLevelParams
    {
        [SerializeField] private TowerLevelRegularParamsMarker levelRegularParams;
        [SerializeField] private TowerLevelAbilitiesParams activeLevelAbilities;
        [SerializeField] private TowerLevelAbilitiesParams towersInAreaLevelAbilities;
        [SerializeField] private TowerLevelAbilitiesParams passiveLevelAbilities;
        
        [Space]
        [SerializeField] private ProjectileView projectilePrefab;

        public TowerLevelRegularParamsMarker LevelRegularParams
        {
            get => levelRegularParams;
#if UNITY_EDITOR
            set => levelRegularParams = value;
#endif
        }
        
        public TowerLevelAbilitiesParams ActiveLevelAbilities
        {
            get => activeLevelAbilities;
#if UNITY_EDITOR
            set => activeLevelAbilities = value;
#endif
        }

        public TowerLevelAbilitiesParams TowersInAreaAbilities
        {
            get => towersInAreaLevelAbilities;
#if UNITY_EDITOR
            set => towersInAreaLevelAbilities = value;
#endif
        }

        public TowerLevelAbilitiesParams PassiveLevelAbilities
        {
            get => passiveLevelAbilities;
#if UNITY_EDITOR
            set => passiveLevelAbilities = value;
#endif
        }

        public ProjectileView ProjectilePrefab
        {
            get => projectilePrefab;
#if UNITY_EDITOR
            set => projectilePrefab = value;
#endif
        }
        
        public static class FieldNames
        {
            public static string LevelRegularParams => nameof(levelRegularParams);
            public static string ActiveAbilities => nameof(activeLevelAbilities);
            public static string TowersInAreaAbilities => nameof(towersInAreaLevelAbilities);
            public static string PassiveAbilities => nameof(passiveLevelAbilities);
            public static string ProjectilePrefab => nameof(projectilePrefab);
        }
        
#if UNITY_EDITOR
        public static int GetHeightInInspector(SerializedProperty property)
        {
            int propertyHeight = TowerLevelAbilitiesParams.AbilitiesLabelHeight;
            
            // level regular params
            SerializedProperty levelParamsProperty = property.FindPropertyRelative(FieldNames.LevelRegularParams);
            propertyHeight += AbstractMarker.ReferenceHeight +
                               (((TowerLevelRegularParamsMarker) levelParamsProperty.objectReferenceValue)?.GetHeightInInspector() ?? 0);
            
            // active abilities
            SerializedProperty activeAbilitiesProperty = property.FindPropertyRelative(FieldNames.ActiveAbilities);
            propertyHeight += TowerLevelAbilitiesParams.GetPropertyHeight(activeAbilitiesProperty);
            
            // towers in area abilities
            SerializedProperty towersInAreaAbilitiesProperty = property.FindPropertyRelative(FieldNames.TowersInAreaAbilities);
            propertyHeight += TowerLevelAbilitiesParams.GetPropertyHeight(towersInAreaAbilitiesProperty);
            
            // passive abilities
            SerializedProperty passiveAbilitiesProperty = property.FindPropertyRelative(FieldNames.PassiveAbilities);
            propertyHeight += TowerLevelAbilitiesParams.GetPropertyHeight(passiveAbilitiesProperty);

            // projectile
            propertyHeight += AbstractMarker.ReferenceHeight;

            return propertyHeight;
        }
        
        public static void DrawInInspector(Rect position, SerializedProperty property, bool isUnfolded)
        {
            SerializedProperty levelParamsProperty = property.FindPropertyRelative(FieldNames.LevelRegularParams);
            
            float currentDelta = 0;
            
            if (!isUnfolded)
                return;

            Rect levelParamsPropertyRect = position;
            levelParamsPropertyRect.height = AbstractMarker.ReferenceHeight + ((TowerLevelRegularParamsMarker)levelParamsProperty.objectReferenceValue).GetHeightInInspector();
            levelParamsPropertyRect.y = position.y + currentDelta;
            GUI.BeginGroup(levelParamsPropertyRect, new GUIStyle("box"));
            {
                Rect levelParamsRect = levelParamsPropertyRect;
                levelParamsRect.x = 0;
                levelParamsRect.y = 0;
                ((AbstractMarker)levelParamsProperty.objectReferenceValue).DrawInInspector(levelParamsRect, levelParamsProperty);
                currentDelta += levelParamsPropertyRect.height;
            }
            GUI.EndGroup();
            
            // projectile
            SerializedProperty projectileProperty = property.FindPropertyRelative(FieldNames.ProjectilePrefab);
            Rect projectileRect = position;
            projectileRect.height = AbstractMarker.ReferenceHeight;
            projectileRect.y = position.y + currentDelta;
            projectileProperty.objectReferenceValue = EditorGUI.ObjectField(projectileRect, FieldNames.ProjectilePrefab,
                projectileProperty.objectReferenceValue,
                typeof(ProjectileView), false);
            currentDelta += projectileRect.height;
             
            // active abilities
            SerializedProperty activeAbilitiesArray = property.FindPropertyRelative(FieldNames.ActiveAbilities);
            Rect activeAbilitiesArrayRect = position;
            activeAbilitiesArrayRect.height = TowerLevelAbilitiesParams.GetPropertyHeight(activeAbilitiesArray);
            activeAbilitiesArrayRect.y = position.y + currentDelta;
            TowerLevelAbilitiesParams.DrawInInspector(activeAbilitiesArrayRect, activeAbilitiesArray, true);
            currentDelta += activeAbilitiesArrayRect.height;
            
            // towers in area abilities
            SerializedProperty towersInAreaAbilitiesArray = property.FindPropertyRelative(FieldNames.TowersInAreaAbilities);
            Rect towersInAreaAbilitiesArrayRect = position;
            towersInAreaAbilitiesArrayRect.height = TowerLevelAbilitiesParams.GetPropertyHeight(towersInAreaAbilitiesArray);
            towersInAreaAbilitiesArrayRect.y = position.y + currentDelta;
            TowerLevelAbilitiesParams.DrawInInspector(towersInAreaAbilitiesArrayRect, towersInAreaAbilitiesArray, false);
            currentDelta += towersInAreaAbilitiesArrayRect.height;
            
            // passive abilities
            SerializedProperty passiveAbilitiesArray = property.FindPropertyRelative(FieldNames.PassiveAbilities);
            Rect passiveAbilitiesArrayRect = position;
            passiveAbilitiesArrayRect.height = TowerLevelAbilitiesParams.GetPropertyHeight(passiveAbilitiesArray);
            passiveAbilitiesArrayRect.y = position.y + currentDelta;
            TowerLevelAbilitiesParams.DrawInInspector(passiveAbilitiesArrayRect, passiveAbilitiesArray, false);
        }
#endif
    }
}