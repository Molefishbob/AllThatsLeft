using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployControlledBots : MonoBehaviour
{
    [SerializeField]
    private string _deployBotButton = "Deploy Bot";
    [SerializeField]
    private Transform _deployTarget = null;
    [SerializeField]
    private LayerMask _deployableTerrain = (1 << 12) + (1 << 13);
    [SerializeField]
    private float _deployHeightRange = 1.0f;
    [SerializeField]
    private string _animatorTriggerDeploy = "Deploy";
    [SerializeField]
    private float _deployDelay = 1.0f;

    private Vector3 _deployStartPosition;
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
        _deployStartPosition = _deployTarget.localPosition;
        PrefsManager.Instance.OnBotsUnlockedChanged += ActivateScript;
        ActivateScript(PrefsManager.Instance.BotsUnlocked);
        _timer.OnTimerCompleted += ActivateBot;
    }

    private void OnDestroy()
    {
        if (PrefsManager.Instance != null)
        {
            PrefsManager.Instance.OnBotsUnlockedChanged -= ActivateScript;
        }
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= ActivateBot;
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
            RaycastHit hit;
            Vector3 upVector = -Physics.gravity.normalized;
            if (Physics.Raycast(
                    _deployTarget.parent.TransformPoint(_deployStartPosition) + upVector * _deployHeightRange,
                    Physics.gravity,
                    out hit,
                    2 * _deployHeightRange,
                    _deployableTerrain))
            {
                _deployTarget.position = hit.point;
                DeployBot();
            }
        }
    }

    private void DeployBot()
    {
        _player.ControlsDisabled = true;
        _player._animator?.SetTrigger(_animatorTriggerDeploy);

        _activeBot = GameManager.Instance.BotPool.GetObject();
        _activeBot.transform.position = _deployTarget.position;
        _activeBot.transform.rotation = _deployTarget.rotation;
        _activeBot.ControlsDisabled = true;
        _activeBot.SetControllerActive(true);
        GameManager.Instance.Camera.GetNewTarget(_activeBot.transform, _deployDelay, false);
        _timer.StartTimer(_deployDelay);
    }

    private void ActivateBot()
    {
        _activeBot.ControlsDisabled = false;
        _activeBot = null;
    }

    private void ActivateScript(bool unlock)
    {
        enabled = unlock;
    }
}