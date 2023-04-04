using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CameraSettingsConfig", menuName = "Configs/CameraSettingsConfig", order = 1)]
    public class CameraSettingsConfig : ScriptableObject
    {
        [Header("Camera movement")]   
        [SerializeField] private float movementVelocity;
        [SerializeField] private float verticalSpeed;	

		
        [Header("Camera pitch rotation")]
        [SerializeField] private float pitchRotationSpeed;
        [SerializeField] private float minPitch;
        
        [Header("Camera yaw rotation")]
        [SerializeField] private float yawRotationSpeed;

        public float MovementVelocity => movementVelocity;
        public float VerticalSpeed => verticalSpeed;
        public float PitchRotationSpeed => pitchRotationSpeed;
        public float MinPitch => minPitch;
        public float YawRotationSpeed => yawRotationSpeed;
    }
}