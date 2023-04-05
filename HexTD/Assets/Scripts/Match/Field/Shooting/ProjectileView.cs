using Tools;
using UnityEngine;

namespace Match.Field.Shooting
{
    public class ProjectileView : BaseMonoBehaviour
    {
        [SerializeField] private SpriteRenderer projectileSprite;
        [SerializeField] private Transform splashTransform;
        [SerializeField] private Animator splashAnimator;

        public SpriteRenderer ProjectileSprite => projectileSprite;
        public Transform SplashTransform => splashTransform;
        public Animator SplashAnimator => splashAnimator;
    }
}