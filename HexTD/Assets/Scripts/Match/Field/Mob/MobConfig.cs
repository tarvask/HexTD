using UnityEngine;

namespace Match.Field.Mob
{
    [CreateAssetMenu(fileName = "MobConfig", menuName = "Configs/Match/Mob")]
    public class MobConfig : ScriptableObject
    {
        [SerializeField] private MobView view;
        [SerializeField] private Sprite icon;
        [SerializeField] private MobParameters parameters;

        public MobView View => view;
        public Sprite Icon => icon;
        public MobParameters Parameters => parameters;
    }
}