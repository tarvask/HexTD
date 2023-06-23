using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuCameraController : MonoBehaviour
{
    private const float MovementMultiplier = 1f;

    public Action InputStart;

    private Coroutine _inputCoroutine;

    public bool IsMoved { get; private set; }
    public float CurrentLerpDiff { get; private set; }
    private float _startLerpValue;
    private int _touchID = -1;

    private void Awake()
    {
        LaunchInputCoroutine();
    }

    private void OnDestroy()
    {
        StopInputCoroutine();
    }

    private void LaunchInputCoroutine()
    {
        _inputCoroutine = StartCoroutine(InputCoroutine());
    }

    private void StopInputCoroutine()
    {
        StopCoroutine(_inputCoroutine);
    }

    private IEnumerator InputCoroutine()
    {
        while (true)
        {
            if (Input.touchCount > 0 && _touchID == -1)
            {
                Touch touch = Input.GetTouch(0);
                _touchID = touch.fingerId;
            }

            if (_touchID != -1)
            {
                if (Input.touchCount <= _touchID)
                {
                    //_testText.text = "Wrong finger";
                    _touchID = -1;
                }
                else
                {
                    Touch touch = Input.GetTouch(_touchID);

                    if (touch.phase == TouchPhase.Began && !IsMouseOverUIWithIgnore())
                    {
                        InputStart?.Invoke();
                        _startLerpValue = touch.position.x / Screen.width;
                        CurrentLerpDiff = (touch.position.x / Screen.width - _startLerpValue) * MovementMultiplier;
                        //Debug.Log($"[JoystickDetector][InputCoroutine] Start lerp vslue is {_startLerpValue}");
                        IsMoved = true;
                    }
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        CurrentLerpDiff = (touch.position.x / Screen.width - _startLerpValue) * MovementMultiplier;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        IsMoved = false;
                        _touchID = -1;
                    }
                    else
                    {
                        CurrentLerpDiff = 0;
                    }
                }
            }

            yield return null;
        }
    }

    private bool IsMouseOverUIWithIgnore()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<MouseClickIgnore>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count > 0;
    }
}