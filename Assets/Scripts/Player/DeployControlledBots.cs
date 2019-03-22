using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployControlledBots : MonoBehaviour, IPauseable
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

    private bool _paused = false;
    private Vector3 _deployStartPosition;
    private PlayerMovement _player;

    private void Awake()
    {
        _player = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);

        _deployStartPosition = _deployTarget.localPosition;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!_paused)
        {
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
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void DeployBot()
    {
        _player.ControlsDisabled = true;
        _player._animator?.SetTrigger(_animatorTriggerDeploy);

        PlayerBotInteractions bot = GameManager.Instance.BotPool.GetObject();
        bot.transform.position = _deployTarget.position;
        bot.transform.rotation = _deployTarget.rotation;
        bot._bActive = true;
        GameManager.Instance.Camera.GetNewTarget(bot.transform);
    }
}