using TMPro;
using Tools;
using UnityEngine;

namespace Match.Field.Tower
{
    [RequireComponent(typeof(TowerRegularParametersMarker))]
    public class TowerView : BaseMonoBehaviour
    {
        private float _defaultZoneScale;
        
        [SerializeField] private TowerLevelView[] levels;
        [SerializeField] private TextMeshPro typeText;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private GameObject constructingGo;
        [SerializeField] private GameObject removingGo;
        [SerializeField] private SpriteRenderer[] zones;

#if UNITY_EDITOR
        public TowerLevelView[] Levels => levels;
#endif

        public void SetType(string towerType)
        {
            typeText.text = towerType;
        }

        public void SetLevel(int newLevel)
        {
            for (int levelIndex = 0; levelIndex < levels.Length; levelIndex++)
            {
                levels[levelIndex].SetActive(newLevel == levelIndex + 1);
            }
            
            background.material.color = new Color(1f, 1f,1f, 1f);
            constructingGo.SetActive(false);
        }

        public void SetRemoving()
        {
            foreach (TowerLevelView towerLevel in levels)
            {
                towerLevel.gameObject.SetActive(false);
            }
            
            background.material.color = new Color(1f, 1f,1f, 0.5f);
            removingGo.SetActive(true);
        }
        
        public void SetConstructing()
        {
            foreach (TowerLevelView towerLevel in levels)
            {
                towerLevel.gameObject.SetActive(false);
            }
            
            background.material.color = new Color(1f, 1f,1f, 0.5f);
            constructingGo.SetActive(true);
        }

        public void ShowZones(float[] zoneSizes)
        {
            _defaultZoneScale = 1f / zones[0].sprite.texture.width * 2;
            for (int zoneIndex = 0; zoneIndex < zones.Length && zoneIndex < zoneSizes.Length; zoneIndex++)
            {
                zones[zoneIndex].transform.localScale = _defaultZoneScale * zoneSizes[zoneIndex] * Vector2.one;
                zones[zoneIndex].gameObject.SetActive(true);
            }
        }

        public void HideZones()
        {
            for (int zoneIndex = 0; zoneIndex < zones.Length; zoneIndex++)
            {
                zones[zoneIndex].transform.localScale = _defaultZoneScale * Vector2.one;
                zones[zoneIndex].gameObject.SetActive(false);
            }
        }
    }
}