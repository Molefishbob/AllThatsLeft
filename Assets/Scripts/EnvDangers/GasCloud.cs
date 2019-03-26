using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    [SerializeField, Tooltip("Time until the unit dies inside the area")]
    private float _timeUntilOof = 2;

    private void OnTriggerEnter(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        unit?.EnterGas(_timeUntilOof);
    }

    private void OnTriggerExit(Collider other)
    {
        GassableUnit unit = other.GetComponent<GassableUnit>();
        unit?.ExitGas();
    }
}
