using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float _verticalSensitivity;
    public float _horizontalSensitivity;

    private float _yaw = 0.0f;
    private float _pitch = 0.0f;


    void Update()
    {
        _yaw -= _verticalSensitivity * Input.GetAxis("Mouse Y");
        _pitch += _horizontalSensitivity * Input.GetAxis("Mouse X");
       
        transform.eulerAngles = new Vector3(_yaw, _pitch, 0.0f);
    }
}
