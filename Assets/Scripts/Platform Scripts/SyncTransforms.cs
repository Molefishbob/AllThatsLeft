using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransforms : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused) return;

        Physics.SyncTransforms();
    }
}