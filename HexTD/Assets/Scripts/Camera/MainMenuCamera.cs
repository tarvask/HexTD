using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    private const float LeftMaxPoint = -20f;
    private const float RightMaxPoint = 9f;

    private float _startLerpPoint;

    [SerializeField] private MainMenuCameraController _movable;

    private void Awake()
    {
        _movable.InputStart += StartMovementHandler;
    }

    private void StartMovementHandler()
    {
        _startLerpPoint = (-transform.position.x - LeftMaxPoint) / (RightMaxPoint - LeftMaxPoint);
    }

    private void FixedUpdate()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        if (_movable.IsMoved)
        {
            var lerpValue = _startLerpPoint + _movable.CurrentLerpDiff;
            float lerpPosX = (Mathf.Lerp(LeftMaxPoint, RightMaxPoint, lerpValue));

            transform.position = new Vector3(-lerpPosX, transform.position.y, transform.position.z);
        }
    }

    private void OnDestroy()
    {
        _movable.InputStart -= StartMovementHandler;
    }
}
