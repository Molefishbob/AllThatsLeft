using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour, IPauseable
{
    public LayerMask _ignoredLayer;
    public Transform _lookAt;
    public Transform _camTransform;
    public float _distance = 5.0f;
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public  float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    private bool _paused;

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camTransform = transform;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }
    
    // Update is called once per frame
    private void Update()
    {        
        _yaw += _horizontalSensitivity * Input.GetAxis("Mouse X");
        _pitch -= _verticalSensitivity * Input.GetAxis("Mouse Y");
        
        if (_pitch > 85)
        {
            _pitch = 85;
        }
        else if (_pitch < -70)
        {
            _pitch = -70;
        }
 
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        RaycastHit hit;
        float tempDistance = _distance;
        if (Physics.Raycast(_lookAt.position, transform.TransformDirection(Vector3.back), out hit, _distance, ~_ignoredLayer))
        {
            Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tempDistance = newDistance;
        }
        Vector3 dir = new Vector3(0, 0, -tempDistance);
        _camTransform.position = _lookAt.position + rotation * dir;
        _camTransform.LookAt(_lookAt.position);
    }
}