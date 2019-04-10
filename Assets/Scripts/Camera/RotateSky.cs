using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 0.4f;

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;

        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.World);
    }
}
