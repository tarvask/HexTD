using UnityEngine;

namespace Match.Field.VFX.VfxSubControllers
{
    public class RotatorVfxSubController : BaseVfxSubController
    {
        [SerializeField] private Transform rotationObject;
        [SerializeField] private float high;
        [SerializeField] private float radius;
        [SerializeField] private float circleDuration;

        private float _timer;

        private void Start()
        {
            if(rotationObject.parent != transform)
                rotationObject.parent = transform;
            
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            UpdateObjectPosition();
        }

        public override void Play()
        {
            UpdateObjectPosition();
            gameObject.SetActive(true);
        }

        public override void Stop()
        {
            _timer = 0f;
            UpdateObjectPosition();
            gameObject.SetActive(false);
        }

        private void UpdateObjectPosition()
        {
            float angle = 2 * Mathf.PI * (_timer / circleDuration);
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            rotationObject.localPosition = new Vector3(radius*cos, high, radius * sin);
        }
    }
}