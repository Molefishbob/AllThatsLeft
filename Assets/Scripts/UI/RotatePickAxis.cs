using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePickAxis : MonoBehaviour
{
    public float speed;
    private Quaternion Rotation;
    private Vector3 RotationVector;
    public enum iAxis
    {
        Up,
        Forward,
        Right
    }
    public iAxis m_iDirection = iAxis.Up;

    private void Update()
    {
        switch (m_iDirection)
        {
            case iAxis.Up:
                transform.Rotate(Vector3.up, speed * Time.unscaledDeltaTime);
                break;
            case iAxis.Forward:
                transform.Rotate(Vector3.forward, speed * Time.unscaledDeltaTime);
                break;
            case iAxis.Right:
                transform.Rotate(Vector3.right, speed * Time.unscaledDeltaTime);
                break;
            default:
                break;
        }
    }
}
