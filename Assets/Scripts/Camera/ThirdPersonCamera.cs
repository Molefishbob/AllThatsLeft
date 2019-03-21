using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour, IPauseable
{
    public LayerMask _groundLayer;
    private Transform _lookAt, _oldTarget;
    public float _distance = 10.0f;
    public float _maxDistance = 15.0f;
    public float _minDistance = 5.0f;
    public float _zoomSpeed = 0.5f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    [Tooltip("How low the camera can go")]
    public float _minPitch = -70;
    [Tooltip("How high the camera can go")]
    public float _maxPitch = 85;
    [Tooltip("How fast the camera moves to the new target")]
    public float _targetToTargetSpeed = 1;
    private float _lerperHelper = 0;
    private bool _paused;
    private bool _movingToTarget;
    private float _newDistance;

    [SerializeField]
    private string _cameraTargetName = "CameraTarget";

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
        GetInstantNewTarget(GameManager.Instance.Player.transform.Find(_cameraTargetName));
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
        if (!_movingToTarget)
        {
            _distance += (Input.GetAxis("Scroll")) * _zoomSpeed;

            if(_distance < _minDistance)
            {
                _distance = _minDistance;
            }else if (_distance > _maxDistance)
            {
                _distance = _maxDistance;
            }

            _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis);
            _pitch += _verticalSensitivity * Input.GetAxis(_cameraYAxis);

            if (_pitch > _maxPitch)
            {
                _pitch = _maxPitch;
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
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
            Vector3 dir = new Vector3(0, 0, -_newDistance);

            transform.position = (Vector3.Lerp(_oldTarget.position, _lookAt.position, _lerperHelper)) + rotation * dir ;
            _lerperHelper += 0.1f * _targetToTargetSpeed;

            if(_lerperHelper >= 1)
            {
                _movingToTarget = false;
                _lerperHelper = 0;
            }
        }
    }

    private float CheckCollision(float tDistance)
    {

        RaycastHit hit;

        if (Physics.SphereCast(_lookAt.position, 1, transform.TransformDirection(Vector3.back), out hit, _distance, _groundLayer))
        {
            //Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            
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
        Transform tf = trans.Find(_cameraTargetName);
        if (tf != null)
        {
            trans = tf;
        }

        if (trans != _lookAt)
        {
            _oldTarget = _lookAt;
            _lookAt = trans;
            _movingToTarget = true;
        }
        if(_oldTarget == null)
        {
            _oldTarget = trans;
        }
    }

    public void GetInstantNewTarget(Transform trans)
    {
        Transform tf = trans.Find(_cameraTargetName);
        if (tf != null)
        {
            trans = tf;
        }

        _lookAt = trans;  
    }
}
