using UnityEditor;
using UnityEngine;

namespace Match.Field.Tower
{
    public class TowerLevelRegularParamsMarker : AbstractMarker
    {
        protected override int NumberOfFields => 8;
        
        [SerializeField] private TowerLevelRegularParams data;

        public TowerLevelRegularParams Data => data;
        
#if UNITY_EDITOR
        public override void DrawInInspector(Rect position, SerializedProperty property)
        {
            // reference
            base.DrawInInspector(position, property);

            // properties
            Rect levelPropertyRect = position;
            levelPropertyRect.height = PropertyHeight;
            levelPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 0;
            DrawProperty(levelPropertyRect, TowerLevelRegularParams.FieldNames.Level, $"{data.Level}");
            
            Rect attackPowerPropertyRect = position;
            attackPowerPropertyRect.height = PropertyHeight;
            attackPowerPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 1;
            DrawProperty(attackPowerPropertyRect, TowerLevelRegularParams.FieldNames.AttackPower, $"{data.AttackPower}");
            
            Rect reloadTimePropertyRect = position;
            reloadTimePropertyRect.height = PropertyHeight;
            reloadTimePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 2;
            DrawProperty(reloadTimePropertyRect, TowerLevelRegularParams.FieldNames.ReloadTime, $"{data.ReloadTime}");
            
            Rect attackRadiusPropertyRect = position;
            attackRadiusPropertyRect.height = PropertyHeight;
            attackRadiusPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 3;
            DrawProperty(attackRadiusPropertyRect, TowerLevelRegularParams.FieldNames.AttackRadius, $"{data.AttackRadiusInHexCount}");
            
            Rect projectileSpeedPropertyRect = position;
            projectileSpeedPropertyRect.height = PropertyHeight;
            projectileSpeedPropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 4;
            DrawProperty(projectileSpeedPropertyRect, TowerLevelRegularParams.FieldNames.ProjectileSpeed, $"{data.ProjectileSpeed}");
            
            Rect pricePropertyRect = position;
            pricePropertyRect.height = PropertyHeight; 
            pricePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 5;
            DrawProperty(pricePropertyRect, TowerLevelRegularParams.FieldNames.Price, $"{data.Price}");
            
            Rect refundPricePropertyRect = position;
            refundPricePropertyRect.height = PropertyHeight; 
            pricePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 6;
            DrawProperty(refundPricePropertyRect, TowerLevelRegularParams.FieldNames.RefundPrice, $"{data.RefundPrice}");
            
            Rect buildingTimePropertyRect = position;
            buildingTimePropertyRect.height = PropertyHeight; 
            buildingTimePropertyRect.y = position.y + ReferenceHeight + PropertyHeight * 7;
            DrawProperty(buildingTimePropertyRect, TowerLevelRegularParams.FieldNames.BuildingTime, $"{data.BuildingTime}");
        }
#endif
    }
}