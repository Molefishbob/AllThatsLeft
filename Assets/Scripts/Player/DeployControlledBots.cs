using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployControlledBots : MonoBehaviour
{
    public event GenericEvent OnDeployBot;

    [SerializeField]
    private string _deployBotButton = "Deploy Bot";
    public float _throwDistance = 2.0f;
    public float _throwHeight = 1.0f;
    public float _throwTime = 2.0f;
    [SerializeField]
    private string _animatorTriggerDeploy = "Deploy";
    [SerializeField]
    private GameObject _aimingLine = null;

    private MainCharMovement _player;
    //private CharControlPlatformMovement _ccpm;
    private BotMovement _activeBot;
    private ScaledOneShotTimer _timer;
    private bool _paused = true;
    private bool _holding = false;
    private bool _hit = false;
    //private Vector3 _platformAdd = Vector3.zero;

    private void Awake()
    {
        _player = GetComponent<MainCharMovement>();
        //_ccpm = GetComponent<CharControlPlatformMovement>();
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Update()
    {
        if (!PrefsManager.Instance.BotsUnlocked) return;
        if (GameManager.Instance.GamePaused)
        {
            _paused = true;
            return;
        }
        if (_paused)
        {
            _paused = false;
            return;
        }

        if (_player.IsGrounded)
        {
            if (!_player.ControlsDisabled && Input.GetButtonDown(_deployBotButton))
            {
                _player.HoldPosition = true;
                _holding = true;
                _aimingLine.SetActive(true);
            }

            if (_holding)
            {
                if (Input.GetButtonUp(_deployBotButton))
                {
                    _holding = false;
                    _aimingLine.SetActive(false);
                    _player.ControlsDisabled = true;
                    _player.HoldPosition = false;
                    _player._animator.SetTrigger(_animatorTriggerDeploy);
                    if (OnDeployBot != null) OnDeployBot();
                }
            }
        }
        else if (_holding)
        {
            _holding = false;
            _aimingLine.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!PrefsManager.Instance.BotsUnlocked) return;
        if (GameManager.Instance.GamePaused) return;

        if (_activeBot != null)
        {
            if (_activeBot.Dead)
            {
                _timer.StopTimer();
                _activeBot = null;
            }
            else if (!_activeBot.IsGrounded)
            {
                if ((_activeBot._controller.collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
                {
                    _hit = true;
                }
                if (!_hit)
                {
                    //_activeBot.AddDirectMovement(_activeBot.transform.forward * _throwDistance * Time.deltaTime / _throwTime + _platformAdd);
                    _activeBot.AddDirectMovement(_activeBot.transform.forward * _throwDistance * Time.deltaTime / _throwTime);
                }
            }
            else if (!_timer.IsRunning)
            {
                _activeBot.ControlsDisabled = false;
                _activeBot.Activate();
                _activeBot = null;
            }
        }
    }

    public void DeployBot(Transform hand)
    {
        if (!PrefsManager.Instance.BotsUnlocked) return;
        _activeBot = GameManager.Instance.BotPool.GetObject();
        Vector3 pos = transform.InverseTransformPoint(hand.position);
        pos.x = 0.0f;
        _activeBot.transform.position = transform.TransformPoint(pos);
        _activeBot.transform.rotation = transform.rotation;
        _activeBot.ControlsDisabled = true;
        _activeBot.SetControllerActive(true);
        _activeBot.GetComponent<PlayerJump>().ForceJump(_throwHeight, false);
        _hit = false;
        GameManager.Instance.Camera.MoveToTarget(_activeBot.transform, _throwTime, false);
        //_platformAdd = _ccpm.CurrentMove;
        _timer.StartTimer(_throwTime);
    }
}
