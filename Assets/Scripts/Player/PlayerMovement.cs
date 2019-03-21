using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharControlBase, IDamageReceiver
{
    [SerializeField]
    private string _horizontalAxis = "Horizontal";
    [SerializeField]
    private string _verticalAxis = "Vertical";
    [SerializeField]
    private string _animatorTriggerDeath = "Dying";

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
    private bool _controlsDisabled;

    private PlayerJump _playerJump;

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

        return inputDirection;
    }

    public virtual void TakeDamage(int damage)
    {
        Die();
    }

    public virtual void Die()
    {
        //Debug.Log("Player died");
        //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        _animator?.SetTrigger(_animatorTriggerDeath);
    }
}
