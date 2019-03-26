using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public LayerMask _groundLayer;
    private Transform _lookAt;
    private Vector3 _oldTarget;
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
    private bool _movingToTarget;
    private float _newDistance;
    private int _invertX = 1;
    private int _invertY = 1;
    private Camera cam;
    public int _fieldOfView = 60;

    [SerializeField]
    private string _cameraTargetName = "CameraTarget";

    private void Awake()
    {
        cam = GetComponent<Camera>();
        _newDistance = _distance;
        
    }

    private void Start()
    {
        cam.fieldOfView = _fieldOfView;
    }

    private void OnEnable()
    {
        LockCursor(GameManager.Instance.GamePaused);
        GameManager.Instance.OnGamePauseChanged += LockCursor;

        PrefsManager.Instance.OnInvertedCameraXChanged += ChangeInvertX;
        PrefsManager.Instance.OnInvertedCameraYChanged += ChangeInvertY;
        ChangeInvertX(PrefsManager.Instance.InvertedCameraX);
        ChangeInvertY(PrefsManager.Instance.InvertedCameraY);

        PrefsManager.Instance.OnCameraXSensitivityChanged += SetCameraXSensitivity;
        PrefsManager.Instance.OnCameraYSensitivityChanged += SetCameraYSensitivity;
        SetCameraXSensitivity(PrefsManager.Instance.CameraXSensitivity);
        SetCameraYSensitivity(PrefsManager.Instance.CameraYSensitivity);

        PrefsManager.Instance.OnZoomSpeedChanged += SetZoomSpeed;
        PrefsManager.Instance.OnFieldOfViewChanged += SetFieldOfView;
        SetZoomSpeed(PrefsManager.Instance.ZoomSpeed);
        SetFieldOfView(PrefsManager.Instance.FieldOfView);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePauseChanged -= LockCursor;
        }

        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnInvertedCameraXChanged -= ChangeInvertX;
            PrefsManager.Instance.OnInvertedCameraYChanged -= ChangeInvertY;
            PrefsManager.Instance.OnCameraXSensitivityChanged -= SetCameraXSensitivity;
            PrefsManager.Instance.OnCameraYSensitivityChanged -= SetCameraYSensitivity;
            PrefsManager.Instance.OnZoomSpeedChanged -= SetZoomSpeed;
            PrefsManager.Instance.OnFieldOfViewChanged -= SetFieldOfView;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_lookAt == null) return;

        if (!_movingToTarget)
        {
            _distance += (Input.GetAxis("Scroll")) * _zoomSpeed;

            if (_distance < _minDistance)
            {
                _distance = _minDistance;
            }
            else if (_distance > _maxDistance)
            {
                _distance = _maxDistance;
            }

            _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis) * _invertX;
            _pitch += _verticalSensitivity * Input.GetAxis(_cameraYAxis) * _invertY;

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

            transform.position = (Vector3.Lerp(_oldTarget, _lookAt.position, _lerperHelper)) + rotation * dir;
            _lerperHelper += 0.1f * _targetToTargetSpeed;

            if (_lerperHelper >= 1)
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
            _oldTarget = _lookAt.position;
            _lookAt = trans;
            _movingToTarget = true;
        }
        if (_oldTarget == null)
        {
            _oldTarget = trans.position;
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

    private void ChangeInvertX(bool b)
    {
        _invertX = b ? -1 : 1;
    }

    private void ChangeInvertY(bool b)
    {
        _invertY = b ? -1 : 1;
    }

    private void SetCameraXSensitivity(float sens)
    {
        _horizontalSensitivity = sens;
    }

    private void SetCameraYSensitivity(float sens)
    {
        _verticalSensitivity = sens;
    }

    private void LockCursor(bool paused)
    {
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetZoomSpeed(int zSpeed)
    {
        _zoomSpeed = zSpeed;
    }

    private void SetFieldOfView(int fov)
    {
        _fieldOfView = fov;
    }
}
