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

    private Vector3 _deployStartPosition;

    private void Start()
    {
        _deployStartPosition = _deployTarget.localPosition;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

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
                DeployBot();
            }
        }
    }

    private void DeployBot()
    {
        PlayerBotInteractions bot = GameManager.Instance.BotPool.GetObject();
        bot.transform.position = _deployTarget.position;
        bot.transform.rotation = _deployTarget.rotation;
        bot._bActive = true;
        GameManager.Instance.Player.ControlsDisabled = true;
        GameManager.Instance.Player._animator?.SetTrigger(_animatorTriggerDeploy);
        GameManager.Instance.Camera.GetNewTarget(bot.transform);
    }
}