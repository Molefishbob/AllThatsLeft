using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerMovement : CharControlBase
{
    public string _horizontalAxis = "Horizontal";
    public string _verticalAxis = "Vertical";

    private Transform _cameraTransform;

    protected override void Awake()
    {
        base.Awake();
        _cameraTransform = FindObjectOfType<ThirdPersonCam>().transform;
    }

    protected override Vector3 InternalMovement()
    {
        // read input
        float horizontal = Input.GetAxisRaw(_horizontalAxis);
        float vertical = Input.GetAxisRaw(_verticalAxis);

        // create combined vector of input
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // clamp magnitude
        float desiredSpeed = Mathf.Clamp(inputDirection.magnitude, 0.0f, 1.0f);

        // convert to world space relative to camera
        inputDirection = _cameraTransform.TransformDirection(inputDirection);

        // remove pitch
        inputDirection.y = 0;

        // apply magnitude
        inputDirection = inputDirection.normalized * desiredSpeed;

        return inputDirection;
    }
}
