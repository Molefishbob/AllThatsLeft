﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldThirdPersonCam : MonoBehaviour
{
    public LayerMask _wallLayer, _groundLayer;
    private Transform _lookAt;
    public float _distance = 5.0f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    private float _tempDistance, _oldDistance;
    private float _lerpDistance;
    private float _lerperHelper = 0;
    private bool _zooming = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _tempDistance = _distance;
        _lookAt = gameObject.transform.parent;
    }

    private void Update()
    {
        _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis);
        _pitch -= _verticalSensitivity * Input.GetAxis(_cameraYAxis);

        if (_pitch > 85)
        {
            _pitch = 85;
        }
        else if (_pitch < -70)
        {
            _pitch = -70;
        }

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        if (_zooming)
        {
            _lerperHelper += 0.2f;
            _lerpDistance = Mathf.Lerp(_oldDistance, _tempDistance, _lerperHelper);
        }

        if (!_zooming)
        {
            _oldDistance = _tempDistance;
            _tempDistance = CheckCollision(_tempDistance);
            _lerpDistance = Mathf.Lerp(_oldDistance, _tempDistance, _lerperHelper);
        }
        
        if (_lerperHelper > 1.01f)
        {
            
            _zooming = false;
            _lerperHelper = 0;

        }

        Vector3 dir = new Vector3(0, 0, -_lerpDistance);
        
        transform.position = _lookAt.position + rotation * dir;
        transform.LookAt(_lookAt.position);
    }

    private float CheckCollision(float tDistance)
    {
        
        RaycastHit hit;

        if (Physics.Raycast(_lookAt.position, transform.TransformDirection(Vector3.back), out hit, _distance, _wallLayer))
        {
            Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
            _zooming = true;
        } else if (Physics.Raycast(_lookAt.position, transform.TransformDirection(Vector3.back), out hit, _distance, _groundLayer))
        {
            Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
        }
        else
        {
            tDistance = _distance;
            _zooming = true;
        }

        return tDistance;
    }
}