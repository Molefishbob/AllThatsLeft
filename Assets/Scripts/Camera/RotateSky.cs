using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public const float FullCircle = 360.0f;

    [SerializeField]
    private float _rotateSpeed = 0.4f;

    private float _currentRotation = 0.0f;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.fieldOfView = PrefsManager.Instance.FieldOfView;
        GameManager.Instance.Camera._cameras.Add(cam);
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        Vector3 angles = GameManager.Instance.Camera.transform.eulerAngles;
        _currentRotation += _rotateSpeed * Time.deltaTime;
        if (_currentRotation >= FullCircle) _currentRotation -= FullCircle;
        angles.y += _currentRotation;
        transform.rotation = Quaternion.Euler(angles);
    }
}
