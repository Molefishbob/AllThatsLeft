using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoZoomThirdPersonCam : MonoBehaviour, IPauseable
{
    public LayerMask _groundLayer;
    private Transform _lookAt, _oldTarget;
    public float _distance = 5.0f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    [Tooltip("How low the camera can go")]
    public float _minPitch = -70;
    [Tooltip("How high the camera can go")]
    public float maxPitch = 85;
    public float _zoomSpeed = 1;
    private float _lerperHelper = 0;
    private bool _paused;
    private bool _zooming;
    private float _newDistance;

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
        _newDistance = _distance;
    }

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

    private void Update()
    {
        if (!_zooming)
        {
            _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis);
            _pitch -= _verticalSensitivity * Input.GetAxis(_cameraYAxis);

            if (_pitch > maxPitch)
            {
                _pitch = maxPitch;
            }
            else if (_pitch < _minPitch)
            {
                _pitch = _minPitch;
            }

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

            _newDistance = CheckCollision(_newDistance);

            Vector3 dir = new Vector3(0, 0, -_newDistance);

            transform.position = _lookAt.position + rotation * dir;
            transform.LookAt(_lookAt.position);
        }
        else
        {
            transform.position = Vector3.Lerp(_oldTarget.position, _lookAt.position, _lerperHelper);
            _lerperHelper += 0.1f * _zoomSpeed;

            if(_lerperHelper >= 1)
            {
                _zooming = false;
                _lerperHelper = 0;
            }
        }
    }

    private float CheckCollision(float tDistance)
    {

        RaycastHit hit;

        if (Physics.SphereCast(_lookAt.position, 1, transform.TransformDirection(Vector3.back), out hit, _distance, _groundLayer))
        {
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
        }
        else
        {
            tDistance = _distance;
        }

        return tDistance;
    }

    public void GetNewTarget(Transform trans)
    {
        if (trans != _lookAt)
        {
            _oldTarget = _lookAt;
            _lookAt = trans;
            _zooming = true;
        }
        if(_oldTarget == null)
        {
            _oldTarget = trans;
        }
    }

    public void GetInstantNewTarget(Transform trans)
    {
        _lookAt = trans;  
    }
}
