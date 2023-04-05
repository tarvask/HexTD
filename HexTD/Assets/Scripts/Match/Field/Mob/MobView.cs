using TMPro;
using Tools;
using UnityEngine;

namespace Match.Field.Mob
{
    public class MobView : BaseMonoBehaviour
    {
        [SerializeField] private SpriteRenderer mobOutfit;
        [SerializeField] private TextMeshPro[] damageTextItems;

        public SpriteRenderer MobOutfit => mobOutfit;
        public TextMeshPro[] DamageTextItems => damageTextItems;
    }
}