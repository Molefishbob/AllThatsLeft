using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float _verticalSensitivity;

    private float _pitch = 0.0f;

    void Update()
    {
        _pitch -= _verticalSensitivity * Input.GetAxis("Mouse Y");
       
        if(_pitch <= -90)
        {
            _pitch = -90;
        } else if(_pitch >= 90)
        {
            _pitch = 90;
        }

        transform.eulerAngles = new Vector3(_pitch, Movement.Yaw, 0.0f);
    }
}
