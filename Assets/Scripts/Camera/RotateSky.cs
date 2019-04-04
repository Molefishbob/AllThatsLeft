using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public Material _bgMat;

    public float RotateSpeed = 1.2f;
   private void Awake() {
        RenderSettings.skybox = Instantiate(_bgMat);
    }
    void Update()
    {

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);

    }
}
