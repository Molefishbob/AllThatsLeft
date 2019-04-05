using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public float RotateSpeed = 1.2f;

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }

    private void OnDestroy()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
}
