using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField]
    protected string _playerAnimTriggerStart = "Victory";
    [SerializeField]
    protected string _playerAnimTriggerEnd = "Default";
    [SerializeField]
    protected float _holdPoseTime = 3.0f;
    [SerializeField]
    protected GameObject _model = null;
    [SerializeField]
    protected Animator _modelAnimator = null;
    [SerializeField]
    protected string _modelAnimatorTrigger = "Hold";
    [SerializeField]
    protected float _cameraLockPitch = 0;
    [SerializeField]
    protected float _cameraLockDistance = 8;

    protected bool _animate = false;
    protected Collider _collider;
    protected ScaledOneShotTimer _timer;
    protected Transform _hand = null;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    protected virtual void Start()
    {
        _timer.OnTimerCompleted += DoneHolding;
    }

    private void OnDestroy()
    {
        if (_timer != null) _timer.OnTimerCompleted -= DoneHolding;
    }

    private void FixedUpdate()
    {
        if (_animate && GameManager.Instance.Player.IsGrounded)
        {
            GameManager.Instance.Player._animator.SetTrigger(_playerAnimTriggerStart);
            _animate = false;
            GameManager.Instance.Camera.Pitch = _cameraLockPitch;
            GameManager.Instance.Camera.Yaw = GameManager.Instance.Player.transform.eulerAngles.y;
            GameManager.Instance.Camera.MoveToTargetInstant(GameManager.Instance.Player.transform);
            GameManager.Instance.Camera.RefreshValues(_cameraLockDistance);
            GameManager.Instance.Camera.Frozen = true;
            ExtraTrigger();
        }
    }

    private void LateUpdate()
    {
        if (_timer.IsRunning)
        {
            _model.transform.position = _hand.position;
            _model.transform.rotation = GameManager.Instance.Player.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _collider.enabled = false;
        _model.SetActive(false);
        GameManager.Instance.Player.ControlsDisabled = true;
        _animate = true;

        PlayerAnimatorMiddlehand pamh = GameManager.Instance.Player.GetComponentInChildren<PlayerAnimatorMiddlehand>();
        pamh._collectible = this;
    }

    protected virtual void ExtraTrigger() { }

    public void HoldPose(Transform hand)
    {
        _hand = hand;
        _model.SetActive(true);
        _model.transform.position = _hand.position;
        _model.transform.rotation = GameManager.Instance.Player.transform.rotation;
        if (_modelAnimator != null) _modelAnimator.SetTrigger(_modelAnimatorTrigger);
        _timer.StartTimer(_holdPoseTime);
        HoldAction();
    }

    protected virtual void HoldAction() { }

    private void DoneHolding()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
        GameManager.Instance.Player._animator.SetTrigger(_playerAnimTriggerEnd);
        GameManager.Instance.Camera.Frozen = false;
        CollectAction();
        gameObject.SetActive(false);
    }

    protected abstract void CollectAction();
}
