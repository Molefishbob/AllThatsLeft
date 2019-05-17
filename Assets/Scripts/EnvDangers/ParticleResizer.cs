using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleResizer : MonoBehaviour
{
    [SerializeField]
    private float _effectOverflowAmount = 1.0f;
    [SerializeField]
    private float[] _particleDensities = { 2.0f, 0.1f };

    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        ParticleSystem[] effects = GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Stop(false);
            ParticleSystem.ShapeModule shape = effects[i].shape;
            shape.position = box.center;
            shape.scale = box.size + Vector3.one * _effectOverflowAmount;

            ParticleSystem.EmissionModule emission = effects[i].emission;
            emission.rateOverTimeMultiplier = _particleDensities[i] * Mathf.Sqrt(shape.scale.x * shape.scale.y * shape.scale.z);
            effects[i].Play(false);
        }
    }
}
