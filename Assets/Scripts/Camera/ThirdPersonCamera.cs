using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public event GenericEvent OnCameraZoomedByPlayer;
    public event GenericEvent OnCameraMovedByPlayer;

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
    private float _pitch;
    public float _startingPitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    [Tooltip("How low the camera can go")]
    public float _minPitch = -70;
    [Tooltip("How high the camera can go")]
    public float _maxPitch = 85;
    [Tooltip("How fast the camera moves to the new target by default")]
    public float _defaultTransitionTime = 1;
    private float _newDistance;
    private int _invertX = 1;
    private int _invertY = 1;
    private Camera[] _cameras;
    private ScaledOneShotTimer _transitionTimer;
    public float _horSensMulti = 0.05f;
    public float _verSensMulti = 0.05f;
    public float _zoomMulti = 0.01f;
    private bool _follow;
    private float _botDistance;
    private float _playerDistance;
    private bool _canZoom;
    private bool _followingPlayer;
    private bool _lookAtHacked;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;

    [SerializeField]
    private string _cameraTargetName = "CameraTarget";

    private void Awake()
    {
        _cameras = GetComponentsInChildren<Camera>();
        _newDistance = _distance;
        _transitionTimer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        _botDistance = _minDistance;
        OnPlayerRebirth();
    }

    private void OnEnable()
    {
        UnLockCursor(GameManager.Instance.GamePaused);
        GameManager.Instance.OnGamePauseChanged += UnLockCursor;

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

        GameManager.Instance.Player.OnPlayerDeath += OnPlayerDeath;
        GameManager.Instance.Player.OnPlayerAlive += OnPlayerRebirth;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePauseChanged -= UnLockCursor;
            if (GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.OnPlayerDeath -= OnPlayerDeath;
                GameManager.Instance.Player.OnPlayerAlive -= OnPlayerRebirth;
            }
        }
        UnLockCursor(true);

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

        if (_transitionTimer.IsRunning && !_lookAtHacked)
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
            if (_canZoom)
            {
                if (_followingPlayer)
                {
                    _newDistance = Mathf.Lerp(_botDistance, _distance, _transitionTimer.NormalizedTimeElapsed);
                }
                else
                {
                    _newDistance = Mathf.Lerp(_playerDistance, _distance, _transitionTimer.NormalizedTimeElapsed);
                }
            }
            Vector3 dir = new Vector3(0, 0, -_newDistance);
            transform.position = (Vector3.Lerp(_oldTarget, _lookAt.position, _transitionTimer.NormalizedTimeElapsed)) + rotation * dir;
        }
        else if (!GameManager.Instance.Player.Dead && !_lookAtHacked)
        {
            float scroll = Input.GetAxis("Scroll");
            _distance -= scroll * _zoomSpeed;

            if (_distance < _minDistance)
            {
                _distance = _minDistance;
            }
            else if (_distance > _maxDistance)
            {
                _distance = _maxDistance;
            }

            float xInput = Input.GetAxis(_cameraXAxis);
            float yInput = Input.GetAxis(_cameraYAxis);

            _yaw += _horizontalSensitivity * xInput * _invertX;
            _pitch += _verticalSensitivity * yInput * _invertY;

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

            if (_follow)
            {
                transform.position = _lookAt.position + rotation * dir;
            }
            transform.LookAt(_lookAt.position);

            if (OnCameraZoomedByPlayer != null && Mathf.Abs(scroll) >= 0.1f) OnCameraZoomedByPlayer();
            if (OnCameraMovedByPlayer != null && (Mathf.Abs(xInput) >= 0.5f || Mathf.Abs(yInput) >= 0.5f)) OnCameraMovedByPlayer();
        }
        else if (_transitionTimer.IsRunning && _lookAtHacked)
        {
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Lerp(currentRotation, _previousRotation, _transitionTimer.NormalizedTimeElapsed);
            transform.position = (Vector3.Lerp(currentPosition, _previousPosition, _transitionTimer.NormalizedTimeElapsed));            
        }
        else
        {
            transform.LookAt(_lookAt.position);
        }
    }

    private float CheckCollision(float tDistance)
    {

        RaycastHit hit;

        if (Physics.SphereCast(_lookAt.position, 1, transform.TransformDirection(Vector3.back), out hit, _distance, _groundLayer))
        {
            //Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            _canZoom = false;
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
        }
        else
        {
            tDistance = _distance;
            _canZoom = true;
        }

        return tDistance;
    }

    public void GetNewTarget(Transform trans, bool willFollowPlayer)
    {

        GetNewTarget(trans, _defaultTransitionTime, willFollowPlayer);
    }

    public void GetNewTarget(Transform trans, float time, bool willFollowPlayer)
    {
        

        if (willFollowPlayer)
        {
            _followingPlayer = true;
            _botDistance = _distance;
            _distance = _playerDistance;
        }
        else
        {
            _followingPlayer = false;
            _playerDistance = _distance;
            _distance = _botDistance;
        }

        Transform tf = trans.Find(_cameraTargetName);
        if (tf != null)
        {
            trans = tf;
        }

        if (trans != _lookAt)
        {
            _oldTarget = _lookAt.position;
            _lookAt = trans;
            _transitionTimer.StartTimer(time);
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

        if (trans.gameObject.layer == 13 || trans.gameObject.layer == 14)
        {
            _distance = 0;
            _previousPosition = transform.position;
            _previousRotation = transform.rotation;
            transform.position = _lookAt.position;
            transform.rotation = _lookAt.rotation;
            _lookAtHacked = true;
        }
    }

    private void OnPlayerDeath()
    {
        _follow = false;
    }

    private void OnPlayerRebirth()
    {
        _follow = true;
        _pitch = _startingPitch;
        _yaw = GameManager.Instance.Player.transform.eulerAngles.y;
    }

    private void ChangeInvertX(bool b)
    {
        _invertX = b ? -1 : 1;
    }

    private void ChangeInvertY(bool b)
    {
        _invertY = b ? -1 : 1;
    }

    private void SetCameraXSensitivity(int sens)
    {
        _horizontalSensitivity = (float)sens * _horSensMulti;
    }

    private void SetCameraYSensitivity(int sens)
    {
        _verticalSensitivity = (float)sens * _verSensMulti;
    }

    private void UnLockCursor(bool unlocked)
    {
        if (unlocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SetZoomSpeed(int zSpeed)
    {
        _zoomSpeed = (float)zSpeed * _zoomMulti;
    }

    private void SetFieldOfView(int fov)
    {
        foreach (Camera cam in _cameras)
        {
            cam.fieldOfView = fov;
        }
    }
}
