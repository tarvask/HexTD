using UnityEditor;
using UnityEngine;

namespace Match.Field.Tower
{
    public class TowerRegularParametersMarker : AbstractMarker
    {
        protected override int NumberOfFields => 7;
        
        [SerializeField] private TowerRegularParameters data;

        public TowerRegularParameters Data => data;
        
#if UNITY_EDITOR
        public override void DrawInInspector(Rect position, SerializedProperty property)
        {
            // reference
            base.DrawInInspector(position, property);

            // properties
            Rect levelPropertyRect = position;
            levelPropertyRect.height = PropertyHeight;
            levelPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 0;
            DrawProperty(levelPropertyRect, TowerRegularParameters.FieldNames.TowerType, $"{data.TowerType}");
            
            Rect attackPowerPropertyRect = position;
            attackPowerPropertyRect.height = PropertyHeight;
            attackPowerPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 1;
            DrawProperty(attackPowerPropertyRect, TowerRegularParameters.FieldNames.TowerName, $"{data.TowerName}");
            
            Rect reloadTimePropertyRect = position;
            reloadTimePropertyRect.height = PropertyHeight;
            reloadTimePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 2;
            DrawProperty(reloadTimePropertyRect, TowerRegularParameters.FieldNames.RaceType, $"{data.RaceType}");
            
            Rect pricePropertyRect = position;
            pricePropertyRect.height = PropertyHeight; 
            pricePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 3;
            DrawProperty(pricePropertyRect, TowerRegularParameters.FieldNames.TargetFindingTacticType, $"{data.TargetFindingTacticType}");
            
            Rect preferUnbuffedTargetsPropertyRect = position;
            preferUnbuffedTargetsPropertyRect.height = PropertyHeight; 
            preferUnbuffedTargetsPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 4;
            DrawProperty(preferUnbuffedTargetsPropertyRect, TowerRegularParameters.FieldNames.PreferUnbuffedTargets, $"{data.PreferUnbuffedTargets}");
            
            Rect buildingTimePropertyRect = position;
            buildingTimePropertyRect.height = PropertyHeight; 
            buildingTimePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 5;
            DrawProperty(buildingTimePropertyRect, TowerRegularParameters.FieldNames.ResetTargetEveryShot, $"{data.ResetTargetEveryShot}");
            
            Rect maxEnemyBlockedRect = position;
            maxEnemyBlockedRect.height = PropertyHeight; 
            maxEnemyBlockedRect.y = position.y + ReferenceHeight + PropertyHeight * 6;
            DrawProperty(buildingTimePropertyRect, TowerRegularParameters.FieldNames.MaxEnemyBlocked, $"{data.MaxEnemyBlocked}");
        }
#endif
    }
}