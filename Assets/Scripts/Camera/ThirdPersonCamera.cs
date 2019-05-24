using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public event GenericEvent OnCameraZoomedByPlayer;
    public event GenericEvent OnCameraMovedByPlayer;

    private const string OutTrigger = "Out";
    public LayerMask _groundLayer;
    private Transform _lookAt;
    private Vector3 _oldTarget;
    private float _distance;
    private float Distance
    {
        get
        {
            return _distance;
        }
        set
        {
            _distance = Mathf.Clamp(value, _minDistance, _maxDistance);
        }
    }
    public float _maxDistance = 15.0f;
    public float _minDistance = 5.0f;
    public float _zoomSpeed = 0.5f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float Yaw = 0.0f;
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
    [HideInInspector]
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
    private bool _frozen;
    public bool Frozen
    {
        get
        {
            return _frozen;
        }
        set
        {
            _frozen = value;
            if (_frozen)
            {
                _cinematicEffect.gameObject.SetActive(true);
            }
            else if (_cinematicEffect.gameObject.activeSelf)
            {
                _cinematicEffect.SetTrigger(OutTrigger);
            }
        }
    }
    public bool PlayerControlled;
    private float _returnToPlayerTime;

    public Transform LookingAt
    {
        get { return _lookAt; }
    }

    [SerializeField]
    private string _cameraTargetName = "CameraTarget";

    [SerializeField]
    private Animator _cinematicEffect = null;

    private Quaternion _oldRotation;
    private Quaternion _newRotation;
    private Vector3 _oldPos;
    private Vector3 _newPos;
    private Transform _playerLookAt;

    private float Pitch
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
        _playerLookAt = GameManager.Instance.Player.transform.Find(_cameraTargetName);
        OnPlayerRebirth();
        _returnControlsTimer.OnTimerCompleted += ReturnControls;
        _freezeTimer.OnTimerCompleted += UnFreeze;
        _transitionTimer.OnTimerCompleted += FinishTransition;
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

        Frozen = false;
        _lookAt = null;
        _transitionTimer.StopTimer();
        _returnControlsTimer.StopTimer();
        _freezeTimer.StopTimer();
        _cinematicEffect.gameObject.SetActive(false);
        PlayerControlled = false;
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
        if (_transitionTimer != null)
        {
            _transitionTimer.OnTimerCompleted -= FinishTransition;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        if (_lookAt == null) return;

        float scroll = 0;
        float xInput = 0;
        float yInput = 0;

        if (PlayerControlled)
        {
            scroll = Input.GetAxis("Scroll");
            xInput = Input.GetAxis(_cameraXAxis);
            yInput = Input.GetAxis(_cameraYAxis);
            Distance -= scroll * _zoomSpeed;
            if (_lookAt == _playerLookAt)
            {
                _playerDistance = Distance;
            }
            else
            {
                _botDistance = Distance;
            }
            Yaw += _horizontalSensitivity * xInput * _invertX;
            Pitch += _verticalSensitivity * yInput * _invertY;
        }

        Quaternion rot = Quaternion.Euler(Pitch, Yaw, 0);

        if (_transitionTimer.IsRunning)
        {
            if (Frozen)
            {
                transform.position = Vector3.Lerp(_oldPos, _newPos, _transitionTimer.NormalizedTimeElapsed);
                transform.rotation = Quaternion.Lerp(_oldRotation, _newRotation, _transitionTimer.NormalizedTimeElapsed);
            }
            else
            {
                Vector3 dir = Vector3.back * Mathf.Lerp(_oldDistance, _newDistance, _transitionTimer.NormalizedTimeElapsed);
                transform.position = (Vector3.Lerp(_oldTarget, _lookAt.position, _transitionTimer.NormalizedTimeElapsed)) + rot * dir;
                transform.rotation = rot;
            }
        }
        else if (Frozen)
        {
            return;
        }
        else if (_follow)
        {
            transform.position = _lookAt.position + rot * Vector3.back;
            transform.LookAt(_lookAt.position);
            Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), Distance);
            transform.position = _lookAt.position + transform.rotation * dirr;
        }
        else
        {
            transform.LookAt(_lookAt.position);
        }

        if (PlayerControlled)
        {
            if (OnCameraZoomedByPlayer != null && Mathf.Abs(scroll) >= 0.1f) OnCameraZoomedByPlayer();
            if (OnCameraMovedByPlayer != null && (Mathf.Abs(xInput) >= 0.5f || Mathf.Abs(yInput) >= 0.5f)) OnCameraMovedByPlayer();
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
        PlayerControlled = true;
        GameManager.Instance.Player.ControlsDisabled = false;
    }

    private void FinishTransition()
    {
        if (Frozen)
        {
            transform.position = _newPos;
            transform.rotation = _newRotation;
            Pitch = transform.eulerAngles.x;
            Yaw = transform.eulerAngles.y;
        }
        else
        {
            PlayerControlled = true;
        }
    }

    public void MoveToTarget(Transform trans)
    {
        MoveToTarget(trans, _defaultTransitionTime);
    }

    public void MoveToTarget(Transform trans, float time, bool forced)
    {
        Transform target = trans.Find(_cameraTargetName);
        if (target == null)
        {
            target = trans;
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
        }

        if (target == _playerLookAt)
        {
            if (forced)
            {
                _cinematicEffect.SetTrigger(OutTrigger);
            }
            Distance = _playerDistance;
            _returnControlsTimer.StartTimer(time);
        }
        else
        {
            Distance = _botDistance;
        }

        Frozen = false;
        _oldRotation = transform.rotation;
        _newRotation = transform.rotation;
        _oldTarget = _lookAt.position;
        _oldDistance = Vector3.Distance(transform.position, _oldTarget);
        _lookAt = target;
        _newDistance = CheckCollision(transform.TransformDirection(Vector3.back), Distance);
        _transitionTimer.StartTimer(time);
    }

    public void MoveToTarget(Transform trans, float time)
    {
        MoveToTarget(trans, time, false);
    }

    public void MoveToTargetInstant(Transform trans)
    {
        Transform target = trans.Find(_cameraTargetName);
        if (target == null)
        {
            target = trans;
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
        }

        if (target == _playerLookAt)
        {
            Distance = _playerDistance;
        }
        else
        {
            Distance = _botDistance;
        }

        _lookAt = target;
        _transitionTimer.StopTimer();
        _returnControlsTimer.StopTimer();
        _freezeTimer.StopTimer();
        Frozen = false;
        PlayerControlled = false;

        if (_follow)
        {
            Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
            transform.position = _lookAt.position + rotation * Vector3.back;
        }

        transform.LookAt(_lookAt.position);
        Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), Distance);
        transform.position = _lookAt.position + transform.rotation * dirr;
    }

    public void MoveToTargetAndLock(Transform trans, float distance, float pitch, float yaw)
    {
        Transform tf = trans.Find(_cameraTargetName);
        if (tf == null)
        {
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
            tf = trans;
        }
        MoveToTargetAndLock(tf.position, distance, pitch, yaw);
    }

    public void MoveToTargetAndLock(Vector3 target, float distance, float pitch, float yaw)
    {
        PlayerControlled = false;
        Frozen = true;
        Pitch = pitch;
        Yaw = yaw;
        Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
        transform.position = target + rotation * Vector3.back;
        transform.LookAt(target);
        Vector3 dirr = Vector3.back * CheckCollision(transform.TransformDirection(Vector3.back), distance);
        transform.position = target + transform.rotation * dirr;
    }

    public void MoveToHackTarget(Transform trans, float lookatTime, float transitionTime)
    {
        _returnToPlayerTime = transitionTime;
        Transform tf = trans.Find(_cameraTargetName);

        if (tf == null)
        {
            Debug.LogWarning("Didn't find camera target on " + trans.gameObject.name);
            tf = trans;
        }

        _lookAt = trans;

        _oldPos = transform.position;
        _oldRotation = transform.rotation;
        _newPos = tf.position;
        _newRotation = Quaternion.LookRotation(trans.position - tf.position);

        Frozen = true;
        PlayerControlled = false;
        _freezeTimer.StartTimer(lookatTime + transitionTime);
        _transitionTimer.StartTimer(transitionTime);
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
        _lookAt = _playerLookAt;
        Distance = _playerDistance;
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
        MoveToTarget(GameManager.Instance.Player.transform, _returnToPlayerTime);
    }
}
