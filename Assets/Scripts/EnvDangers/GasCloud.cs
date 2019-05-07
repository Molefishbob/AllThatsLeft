using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GasCloud : MonoBehaviour
{
    [SerializeField, Tooltip("Time until the unit dies inside the area")]
    private float _timeUntilOof = 2;
    [SerializeField]
    private Color _gassedUnitTint = Color.magenta;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _deathTintStrength = 0.75f;
    [SerializeField]
    private SingleSFXSound _gasSound = null;
    [SerializeField]
    private float _effectOverflowAmount = 1.0f;
    [SerializeField]
    private float _particleDensity = 1.0f;

    private void Start()
    {
        _gasSound.PlaySound();

    }

    private void LateUpdate() {
        BoxCollider box = GetComponent<BoxCollider>();
        ParticleSystem[] effects = GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem effect in effects)
        {
            ParticleSystem.ShapeModule shape = effect.shape;
            shape.position = box.center;
            shape.scale = box.size + Vector3.one * _effectOverflowAmount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        if (unit != null) unit.EnterGas(_timeUntilOof, _gassedUnitTint, _deathTintStrength);
    }

    private void OnTriggerExit(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        if (unit != null) unit.ExitGas();
    }
}
