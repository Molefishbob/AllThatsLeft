using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        _gasSound.PlaySound();
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
