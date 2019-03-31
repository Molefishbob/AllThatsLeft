using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployControlledBots : MonoBehaviour
{
    [SerializeField]
    private string _deployBotButton = "Deploy Bot";
    [SerializeField]
    private Transform _characterHand = null;
    [SerializeField]
    private float _throwDistance = 2.0f;
    [SerializeField]
    private float _throwHeight = 1.0f;
    [SerializeField]
    private float _throwTime = 2.0f;
    [SerializeField]
    private string _animatorTriggerDeploy = "Deploy";

    private MainCharMovement _player;
    private BotMovement _activeBot;
    private ScaledOneShotTimer _timer;
    private bool _paused = true;

    private void Awake()
    {
        _player = GetComponent<MainCharMovement>();
        _timer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        PrefsManager.Instance.OnBotsUnlockedChanged += ActivateScript;
        ActivateScript(PrefsManager.Instance.BotsUnlocked);
    }

    private void OnDestroy()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnBotsUnlockedChanged -= ActivateScript;
        }
    }

    private void Update()
    {
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

        if (!_player.ControlsDisabled && Input.GetButtonDown(_deployBotButton) && _player.IsGrounded)
        {
            _player.ControlsDisabled = true;
            _player._animator?.SetTrigger(_animatorTriggerDeploy);
        }
    }

    private void FixedUpdate()
    {
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
                _activeBot.AddDirectMovement(_activeBot.transform.forward * _throwDistance * Time.deltaTime / _throwTime);
            }
            else if (!_timer.IsRunning)
            {
                _activeBot.ControlsDisabled = false;
                _activeBot = null;
            }
        }
    }

    public void DeployBot()
    {
        _activeBot = GameManager.Instance.BotPool.GetObject();
        Vector3 pos = transform.InverseTransformPoint(_characterHand.position);
        pos.x = 0.0f;
        _activeBot.transform.position = transform.TransformPoint(pos);
        _activeBot.transform.rotation = transform.rotation;
        _activeBot.ControlsDisabled = true;
        _activeBot.SetControllerActive(true);
        _activeBot.GetComponent<PlayerJump>().ForceJump(_throwHeight);
        GameManager.Instance.Camera.GetNewTarget(_activeBot.transform, _throwTime, false);

        _timer.StartTimer(_throwTime);
    }

    private void ActivateScript(bool unlock)
    {
        enabled = unlock;
    }
}