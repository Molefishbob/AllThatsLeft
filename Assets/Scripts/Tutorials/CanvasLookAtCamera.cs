using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        transform.LookAt(transform.position + GameManager.Instance.Camera.transform.rotation * Vector3.forward, GameManager.Instance.Camera.transform.rotation * Vector3.up);
    }
}