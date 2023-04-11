using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.Tower
{
    [CreateAssetMenu(menuName = "Configs/Match/Towers Config")]
    public class TowersConfig : ScriptableObject
    {
        [SerializeField] private List<TowerConfig> towers;
        
        public List<TowerConfig> Towers => towers;

//#if UNITY_EDITOR
//        [ContextMenu("Pull up data from Views to Configs")]
//        public void PullUpDataFromView()
//        {
//            foreach (TowerConfig tower in towers)
//                tower.PullUpDataFromView();
//        }
//#endif
    }
}