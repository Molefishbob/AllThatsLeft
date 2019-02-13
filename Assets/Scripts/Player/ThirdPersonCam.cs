using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{

    public Transform _lookAt;
    public Transform _camTransform;

    private Camera _cam;

    public float _distance = 5.0f;
    private float _currentX = 0.0f;
    private float _currentY = 0.0f;
    public  float _sensitivityX = 2.0f;
    public float _sensitivityY = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        _currentX += Input.GetAxis("Mouse X");
        _currentY += Input.GetAxis("Mouse Y");
        
        if (_currentY > 85)
        {
            _currentY = 85;
        }
        else if (_currentY < -70)
        {
            _currentY = -70;
        }
    }
    void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, - _distance);

        
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        
        _camTransform.position = _lookAt.position + rotation * dir;
        _camTransform.LookAt(_lookAt.position);
    }
}
