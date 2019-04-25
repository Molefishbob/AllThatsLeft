using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    [SerializeField, Tooltip("Time until the unit dies inside the area")]
    private float _timeUntilOof = 2;
    [SerializeField]
    private Color _gassedUnitTint = Color.magenta;

    private void OnTriggerEnter(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        if (unit != null) unit.EnterGas(_timeUntilOof, _gassedUnitTint);
    }

    private void OnTriggerExit(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        if (unit != null) unit.ExitGas();
    }
}
