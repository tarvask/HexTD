using Configs;
using UnityEngine;

namespace InputSystem
{
    public class EditorCameraMovementService
    {
	    private const float MaxCameraPitch = 90;
	    
	    private readonly Camera _editorCamera;
	    private readonly float _verticalSpeed;
	    private readonly float _movementSpeed;
	    private readonly float _pitchRotationSpeed;
	    private readonly float _minPitch;
	    private readonly float _yawRotationSpeed;

	    public EditorCameraMovementService(Camera editorCamera,
		    CameraSettingsConfig cameraSettingsConfig)
	    {
		    _editorCamera = editorCamera;
		    _verticalSpeed = cameraSettingsConfig.VerticalSpeed;
		    _movementSpeed = cameraSettingsConfig.MovementVelocity;
		    _pitchRotationSpeed = cameraSettingsConfig.PitchRotationSpeed;
		    _minPitch = cameraSettingsConfig.MinPitch;
		    _yawRotationSpeed = cameraSettingsConfig.YawRotationSpeed;
	    }
	    
        public void ProcessPossibleCameraMovementInput(float deltaTime)
        {
	        if (Input.GetKey(KeyCode.Q))
		        MoveCameraVertical(_verticalSpeed * deltaTime);
	        else if(Input.GetKey(KeyCode.E))
		        MoveCameraVertical(-_verticalSpeed * deltaTime);

			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				// camera pitch
				if (Input.GetKey(KeyCode.W))
					PitchCameraBy(-_pitchRotationSpeed * deltaTime);
			
				if (Input.GetKey(KeyCode.S))
					PitchCameraBy(_pitchRotationSpeed * deltaTime);
			
				// camera yaw
				if (Input.GetKey(KeyCode.A))
					YawCameraBy(_yawRotationSpeed * deltaTime);
			
				if (Input.GetKey(KeyCode.D))
					YawCameraBy(-_yawRotationSpeed * deltaTime);
			}
			else
			{
				// camera position
				if (Input.GetKey(KeyCode.W))
					MoveCameraForwardOrBackInParallelPlaneBy(_movementSpeed * deltaTime);
			
				if (Input.GetKey(KeyCode.S))
					MoveCameraForwardOrBackInParallelPlaneBy(-_movementSpeed * deltaTime);
			
				if (Input.GetKey(KeyCode.A))
					MoveCameraToSideBy(-_movementSpeed * deltaTime);
			
				if (Input.GetKey(KeyCode.D))
					MoveCameraToSideBy(_movementSpeed * deltaTime);
			}
		}

        private void MoveCameraForwardOrBackInParallelPlaneBy(float relativeValue)
        {
	        _editorCamera.transform.Translate(Vector3.right * relativeValue, Space.World);
        }

        private void MoveCameraToSideBy(float relativeValue)
        {
	        _editorCamera.transform.Translate(Vector3.back * relativeValue, Space.World);
        }

        private void MoveCameraVertical(float relativeValue)
        {
	        _editorCamera.transform.Translate(Vector3.down * relativeValue, Space.World);
        }

        private void PitchCameraBy(float relativeValue)
        {
	        Vector3 currentEuler = _editorCamera.transform.rotation.eulerAngles;
	        float cameraPitch = currentEuler.x + relativeValue;
	        cameraPitch = Mathf.Clamp(cameraPitch, _minPitch, MaxCameraPitch);
	        _editorCamera.transform.rotation = Quaternion.Euler(cameraPitch, currentEuler.y, currentEuler.z);
        }

        private void YawCameraBy(float relativeValue)
        {
	        _editorCamera.transform.RotateAround(_editorCamera.transform.position,Vector3.up,
		        relativeValue);
        }
    }
}