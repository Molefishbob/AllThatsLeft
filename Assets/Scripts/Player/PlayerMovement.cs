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
            if (Jump != null)
            {
                Jump.ControlsDisabled = value;
            }
        }
    }

    [HideInInspector]
    public PlayerJump Jump;

    protected bool _controlsDisabled;

    protected override void Awake()
    {
        base.Awake();
        Jump = GetComponent<PlayerJump>();
    }

    protected override Vector2 InternalMovement()
    {
        if (ControlsDisabled)
        {
            return Vector2.zero;
        }

        // read input
        float horizontal = Input.GetAxis(_horizontalAxis);
        float vertical = Input.GetAxis(_verticalAxis);

        // create combined vector of input
        Vector2 inputDirection = new Vector2(horizontal, vertical);

        inputDirection = RotateInput(inputDirection, GameManager.Instance.Camera.transform.eulerAngles.y);

        // clamp magnitude
        inputDirection = Vector2.ClampMagnitude(inputDirection, 1.0f);

        if (OnPlayerMovement != null && (Mathf.Abs(horizontal) >= 0.5f || Mathf.Abs(vertical) >= 0.5f)) OnPlayerMovement();

        return inputDirection;
    }
}
