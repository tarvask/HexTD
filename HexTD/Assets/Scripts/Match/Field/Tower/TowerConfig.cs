using System.Collections.Generic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower
{
    [CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/Match/Tower")]
    public class TowerConfig : ScriptableObject
    {
        [SerializeField] private TowerView view;
        [SerializeField] private Sprite icon;
        [SerializeField] private TowerParameters parameters;

        public TowerView View => view;
        public Sprite Icon => icon;
        public TowerParameters Parameters => parameters;

#if UNITY_EDITOR
        [ContextMenu("Pull up data from View")]
        public void PullUpDataFromView()
        {
            // copy link to TowerParametersMarker from view to Parameters
            parameters.RegularParameters = view.GetComponent<TowerRegularParametersMarker>();
            int levelsCount = view.Levels.Length;
            
            // save previous projectile prefabs
            ProjectileView[] levelsProjectiles = new ProjectileView[parameters.Levels.Length];

            for (int projectileIndex = 0; projectileIndex < levelsProjectiles.Length; projectileIndex++)
            {
                levelsProjectiles[projectileIndex] = parameters.Levels[projectileIndex].ProjectilePrefab;
            }
            
            parameters.Levels = new TowerLevelParams[view.Levels.Length];

            for (int levelIndex = 0; levelIndex < levelsCount; levelIndex++)
            {
                // copy link to LevelParamsMarker from Level views to Parameters
                parameters.Levels[levelIndex].LevelRegularParams = view.Levels[levelIndex].GetComponent<TowerLevelRegularParamsMarker>();
                
                // try to copy projectile prefab from previous version
                if (levelIndex < levelsProjectiles.Length)
                    parameters.Levels[levelIndex].ProjectilePrefab = levelsProjectiles[levelIndex];
                
                // copy links to AbilityMarker components from Level views to Parameters
                AbstractAbilityMarker[] levelAbilityMarkers = view.Levels[levelIndex].GetComponents<AbstractAbilityMarker>();
                // applied to self
                List<AbstractAbilityMarker> levelPassiveAbilities = new List<AbstractAbilityMarker>(levelAbilityMarkers.Length);
                // applied to target
                List<AbstractAbilityMarker> levelActiveAbilities = new List<AbstractAbilityMarker>(levelAbilityMarkers.Length);
                // applied to towers in area
                List<AbstractAbilityMarker> levelTowersInAreaAbilities = new List<AbstractAbilityMarker>(levelAbilityMarkers.Length);

                // extract passive, active and towers in area abilities
                foreach (AbstractAbilityMarker abilityMarker in levelAbilityMarkers)
                {
                    if (abilityMarker.IsSelfApplied)
                        levelPassiveAbilities.Add(abilityMarker);
                    
                    if (abilityMarker.IsTargetApplied)
                        levelActiveAbilities.Add(abilityMarker);
                    
                    if (abilityMarker.IsTowersInAreaApplied)
                        levelTowersInAreaAbilities.Add(abilityMarker);
                }

                // passive abilities
                parameters.Levels[levelIndex].PassiveLevelAbilities = new TowerLevelAbilitiesParams
                {
                    Abilities = new AbstractAbilityMarker[levelPassiveAbilities.Count]
                };
                for (int abilityIndex = 0; abilityIndex < levelPassiveAbilities.Count; abilityIndex++)
                {
                    parameters.Levels[levelIndex].PassiveLevelAbilities.Abilities[abilityIndex] = levelPassiveAbilities[abilityIndex];
                }

                // active abilities
                parameters.Levels[levelIndex].ActiveLevelAbilities = new TowerLevelAbilitiesParams
                {
                    Abilities = new AbstractAbilityMarker[levelActiveAbilities.Count]
                };
                for (int abilityIndex = 0; abilityIndex < levelActiveAbilities.Count; abilityIndex++)
                {
                    parameters.Levels[levelIndex].ActiveLevelAbilities.Abilities[abilityIndex] = levelActiveAbilities[abilityIndex];
                }
                
                // towers in area abilities
                parameters.Levels[levelIndex].TowersInAreaAbilities = new TowerLevelAbilitiesParams
                {
                    Abilities = new AbstractAbilityMarker[levelTowersInAreaAbilities.Count]
                };
                for (int abilityIndex = 0; abilityIndex < levelTowersInAreaAbilities.Count; abilityIndex++)
                {
                    parameters.Levels[levelIndex].TowersInAreaAbilities.Abilities[abilityIndex] = levelTowersInAreaAbilities[abilityIndex];
                }
            }

            levelsProjectiles = null;
        }
#endif
    }
}