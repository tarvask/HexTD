using Tools;
using UI.ScreenSpaceOverlaySystem;
using UnityEngine;

namespace Match.Field.Tower
{
    public class TowerView : BaseMonoBehaviour, ITargetView
    {
        [SerializeField] private Transform infoPanelPivot;
        public Transform InfoPanelPivot => infoPanelPivot;

        public void SetType(string towerType)
        {
            
        }

        public void SetLevel(int newLevel)
        {
            
        }

        public void SetRemoving()
        {
            
        }
        
        public void SetConstructing()
        {
            
        }

        public void SetPlacing()
        {
            Material material = GetComponentInChildren<MeshRenderer>().material;
            var color = material.color;
            color.a = 0.5f;
            material.color = color;
        }
    }
}