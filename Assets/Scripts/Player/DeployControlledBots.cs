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
    private string _animatorParameterDeploy = "Deploy";

    private bool _shouldDeployBot = false;
    private bool _paused = false;
    private Vector3 _deployStartPosition;

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);

        _deployStartPosition = _deployTarget.localPosition;
    }

    private void Update()
    {
        if (!_paused)
        {
            if (!GameManager.Instance.Player.ControlsDisabled && Input.GetButtonDown(_deployBotButton) && GameManager.Instance.Player.IsGrounded)
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
                    _shouldDeployBot = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_paused)
        {
            if (_shouldDeployBot)
            {
                DeployBot();
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
        _shouldDeployBot = false;
        PlayerBotInteractions bot = GameManager.Instance.BotPool.GetObject();
        bot.transform.position = _deployTarget.position;
        bot.transform.rotation = _deployTarget.rotation;
        GameManager.Instance.Player.ControlsDisabled = true;
        GameManager.Instance.Camera.GetNewTarget(bot.transform);
    }
}