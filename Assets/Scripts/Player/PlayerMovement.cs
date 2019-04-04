using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovement : CharControlBase
{
    public event GenericEvent OnPlayerMovement;

    [SerializeField]
    protected string _horizontalAxis = "Horizontal";
    [SerializeField]
    protected string _verticalAxis = "Vertical";

    [HideInInspector]
    public bool ControlsDisabled
    {
        get
        {
            return _controlsDisabled;
        }
        set
        {
            _controlsDisabled = value;
            if (_playerJump != null)
            {
                _playerJump.ControlsDisabled = value;
            }
        }
    }

    protected bool _controlsDisabled;
    protected PlayerJump _playerJump;

    protected override void Awake()
    {
        base.Awake();
        _playerJump = GetComponent<PlayerJump>();
    }

    protected override Vector3 InternalMovement()
    {
        if (ControlsDisabled)
        {
            return Vector3.zero;
        }

        // read input
        float horizontal = Input.GetAxis(_horizontalAxis);
        float vertical = Input.GetAxis(_verticalAxis);

        // create combined vector of input
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // clamp magnitude
        float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

        // convert to world space relative to camera
        inputDirection = GameManager.Instance.Camera.transform.TransformDirection(inputDirection);

        // remove pitch
        inputDirection.y = 0;

        // apply magnitude
        inputDirection = inputDirection.normalized * desiredSpeed;

        if (OnPlayerMovement != null && (Mathf.Abs(horizontal) >= 0.5f || Mathf.Abs(vertical) >= 0.5f)) OnPlayerMovement();

        return inputDirection;
    }
}
