using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployControlledBots : MonoBehaviour
{
    public event GenericEvent OnDeployBot;

    [SerializeField]
    private string _deployBotButton = "Deploy Bot";
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

        if (!_player.ControlsDisabled && Input.GetButtonDown(_deployBotButton) && _player.IsGrounded)
        {
            _player.ControlsDisabled = true;
            _player._animator?.SetTrigger(_animatorTriggerDeploy);
            if (OnDeployBot != null) OnDeployBot();
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
                _activeBot.AddDirectMovement(_activeBot.transform.forward * _throwDistance * Time.deltaTime / _throwTime);
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
        GameManager.Instance.Camera.MoveToTarget(_activeBot.transform, _throwTime, false);

        _timer.StartTimer(_throwTime);
    }
}