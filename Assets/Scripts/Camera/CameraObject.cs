using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private List<CameraFocus> cameraOptions;
    private int _currentIndex;

    private bool _isTraveling;
    private float _travelTimer;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Start()
    {
        _currentIndex = cameraOptions.Count > 0 ? 0 : -1;

        if (_currentIndex >= 0) SetToTravel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _currentIndex = (_currentIndex + 1) % cameraOptions.Count;
            SetToTravel();
        }

        HandleCameraFocus(cameraOptions[_currentIndex], false);
    }

    private void FixedUpdate()
    {
        HandleCameraFocus(cameraOptions[_currentIndex], true);
    }

    private void SetToTravel()
    {
        _isTraveling = true;
        _travelTimer = 0f;

        _initialPosition = camera.transform.position;
        _initialRotation = camera.transform.rotation;
    }

    private void HandleCameraFocus(CameraFocus focus, bool fixedUpdate)
    {
        if (_isTraveling)
            HandleTravel(focus, fixedUpdate);
        else HandleFocus(focus, fixedUpdate);
    }

    private void HandleTravel(CameraFocus cameraFocus, bool fixedUpdate)
    {
        _travelTimer += Time.deltaTime;
        var t = Mathf.Clamp01(_travelTimer / cameraFocus.travelTime);

        if (fixedUpdate)
        {
            camera.transform.position = Vector3.Lerp(_initialPosition, cameraFocus.target.position, t);
            camera.transform.rotation = Quaternion.Lerp(_initialRotation, cameraFocus.target.rotation, t);
        }

        if (t >= 1) _isTraveling = false;
    }

    private void HandleFocus(CameraFocus cameraFocus, bool fixedUpdate)
    {
        if (cameraFocus.focusTime > 0)
        {
            if (!fixedUpdate) return;
            camera.transform.position = Vector3.Lerp(camera.transform.position, cameraFocus.target.position,
                Time.deltaTime * cameraFocus.focusTime);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, cameraFocus.target.rotation,
                Time.deltaTime * cameraFocus.focusTime);
        }
        else
        {
            if (fixedUpdate) return;
            camera.transform.position = cameraFocus.target.position;
            camera.transform.rotation = cameraFocus.target.rotation;
        }
    }
}

[Serializable]
public class CameraFocus
{
    public Transform target;
    [Min(1)] public float travelTime = 1;
    [Min(0)] public float focusTime;
}