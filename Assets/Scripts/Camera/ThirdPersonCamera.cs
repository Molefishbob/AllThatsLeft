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
    private float _distance = 10.0f;
    public float _maxDistance = 15.0f;
    public float _minDistance = 5.0f;
    public float _zoomSpeed = 0.5f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    [HideInInspector]
    public float Yaw = 0.0f;
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
    public HashSet<Camera> _cameras;
    private ScaledOneShotTimer _transitionTimer;
    private ScaledOneShotTimer _returnControlsTimer;
    private ScaledOneShotTimer _freezeTimer;
    public float _horSensMulti = 0.05f;
    public float _verSensMulti = 0.05f;
    public float _zoomMulti = 0.01f;
    private bool _follow;
    [SerializeField]
    private float _botDistance;
    [SerializeField]
    private float _playerDistance;
    private float _oldDistance;
    public float _collisionRadius = 0.5f;
    public bool Frozen;
    public bool PlayerControlled;
    private float _returnToPlayerTime;

    public Transform LookingAt
    {
        get { return _lookAt; }
    }

    [SerializeField]
    private string _cameraTargetName = "CameraTarget";

    public float Pitch
    {
        get
        {
            return _pitch;
        }
        set
        {
            _pitch = Mathf.Clamp(value, _minPitch, _maxPitch);
        }
    }

    private void Awake()
    {
        _cameras = new HashSet<Camera>(GetComponentsInChildren<Camera>(true));
        _transitionTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _returnControlsTimer = gameObject.AddComponent<ScaledOneShotTimer>();
        _freezeTimer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        OnPlayerRebirth();
        _returnControlsTimer.OnTimerCompleted += ReturnControls;
        _freezeTimer.OnTimerCompleted += UnFreeze;
        PlayerControlled = true;
    }

    private void OnEnable()
    {
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
        OnPlayerRebirth();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.OnPlayerDeath -= OnPlayerDeath;
            GameManager.Instance.Player.OnPlayerAlive -= OnPlayerRebirth;
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

    private void OnDestroy()
    {
        if (_returnControlsTimer != null && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            _returnControlsTimer.OnTimerCompleted -= ReturnControls;
        }
        if (_freezeTimer != null)
        {
            _freezeTimer.OnTimerCompleted -= UnFreeze;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_lookAt == null) return;

        if (Frozen) return;

        if (_transitionTimer.IsRunning)
        {
            Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);

            float lerpedDistance = Mathf.Lerp(_oldDistance, _newDistance, _transitionTimer.NormalizedTimeElapsed);

            Vector3 dir = Vector3.back * lerpedDistance;
            transform.position = (Vector3.Lerp(_oldTarget, _lookAt.position, _transitionTimer.NormalizedTimeElapsed)) + rotation * dir;
        }
        else if (!GameManager.Instance.Player.Dead)
        {
            float scroll = Input.GetAxis("Scroll");
            float xInput = Input.GetAxis(_cameraXAxis);
            float yInput = Input.GetAxis(_cameraYAxis);

            if (PlayerControlled)
            {
                _distance -= scroll * _zoomSpeed;

                if (_distance < _minDistance)
                {
                    _distance = _minDistance;
                }
                else if (_distance > _maxDistance)
                {
                    _distance = _maxDistance;
                }

                Yaw += _horizontalSensitivity * xInput * _invertX;
                Pitch += _verticalSensitivity * yInput * _invertY;
            }

            if (_follow)
            {
                Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
                transform.position = _lookAt.position + rotation * Vector3.back;
            }
            transform.LookAt(_lookAt.position);
            Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), _distance);
            transform.position = _lookAt.position + transform.rotation * dirr;

            if (PlayerControlled)
            {
                if (OnCameraZoomedByPlayer != null && Mathf.Abs(scroll) >= 0.1f) OnCameraZoomedByPlayer();
                if (OnCameraMovedByPlayer != null && (Mathf.Abs(xInput) >= 0.5f || Mathf.Abs(yInput) >= 0.5f)) OnCameraMovedByPlayer();
            }
        }
        else
        {
            transform.LookAt(_lookAt.position);
        }
    }

    private float CheckCollision(Vector3 direction, float distance)
    {
        RaycastHit hit;

        if (Physics.SphereCast(_lookAt.position, _collisionRadius, direction, out hit, distance, _groundLayer))
        {
            //Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            return hit.distance;
        }

        return distance;
    }

    private void ReturnControls()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
    }

    public void MoveToTarget(Transform trans)
    {
        MoveToTarget(trans, _defaultTransitionTime);
    }

    public void MoveToTarget(Transform trans, float time)
    {
        Transform target = trans.Find(_cameraTargetName);
        if (target == null)
        {
            target = trans;
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
        }

        if (target == _lookAt)
        {
            return;
        }

        if (trans == GameManager.Instance.Player.transform)
        {
            _botDistance = _distance;
            _distance = _playerDistance;
            _returnControlsTimer.StartTimer(time);
        }
        else
        {
            _playerDistance = _distance;
            _distance = _botDistance;
        }

        _oldTarget = _lookAt.position;
        _oldDistance = Vector3.Distance(transform.position, _oldTarget);
        _lookAt = target;
        _newDistance = CheckCollision(transform.TransformDirection(Vector3.back), _distance);
        _transitionTimer.StartTimer(time);
    }

    public void MoveToTargetInstant(Transform trans)
    {
        Transform target = trans.Find(_cameraTargetName);
        if (target == null)
        {
            target = trans;
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
        }

        if (target == _lookAt)
        {
            return;
        }

        if (trans == GameManager.Instance.Player.transform)
        {
            _botDistance = _distance;
            _distance = _playerDistance;
        }
        else
        {
            _playerDistance = _distance;
            _distance = _botDistance;
        }

        _lookAt = target;
        _transitionTimer.StopTimer();

        if (_follow)
        {
            Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
            transform.position = _lookAt.position + rotation * Vector3.back;
        }

        transform.LookAt(_lookAt.position);
        Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), _distance);
        transform.position = _lookAt.position + transform.rotation * dirr;
    }

    public void RefreshValues(float distance)
    {
        Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
        transform.position = _lookAt.position + rotation * Vector3.back;
        transform.LookAt(_lookAt.position);
        Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), distance);
        transform.position = _lookAt.position + transform.rotation * dirr;
    }

    public void MoveToHackTargetInstant(Transform trans, float lookatTime, float transitionTime)
    {
        _returnToPlayerTime = transitionTime;
        Transform tf = trans.Find(_cameraTargetName);

        if (tf == null)
        {
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
            tf = trans;
        }

        _lookAt = trans;

        transform.position = tf.position;
        transform.LookAt(trans);

        Pitch = transform.eulerAngles.x;
        Yaw = transform.eulerAngles.y;

        Frozen = true;
        _freezeTimer.StartTimer(lookatTime);
    }

    private void OnPlayerDeath()
    {
        _follow = false;
    }

    public void OnPlayerRebirth()
    {
        _follow = true;
        Pitch = _startingPitch;
        Yaw = GameManager.Instance.Player.transform.eulerAngles.y;
        _lookAt = GameManager.Instance.Player.transform.Find(_cameraTargetName);
        _distance = _playerDistance;
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

    private void UnFreeze()
    {
        Frozen = false;
        MoveToTarget(GameManager.Instance.Player.transform, _returnToPlayerTime);
    }
}
