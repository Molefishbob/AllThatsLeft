using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float _pitch = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        _pitch -= PlayerMovement.Sensitivity * Input.GetAxis("Camera Y");

        if (_pitch <= -90)
        {
            _pitch = -90;
        }
        else if (_pitch >= 90)
        {
            _pitch = 90;
        }

        transform.eulerAngles = new Vector3(_pitch, PlayerMovement.Yaw, 0.0f);
    }


}
