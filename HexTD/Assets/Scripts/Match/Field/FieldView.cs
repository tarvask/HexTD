using UnityEngine;

namespace Match.Field
{
    public class FieldView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;

        public SpriteRenderer Background => background;
    }
}
