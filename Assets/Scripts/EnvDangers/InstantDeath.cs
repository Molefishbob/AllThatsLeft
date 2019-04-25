using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamageReceiver unit = other.GetComponent<IDamageReceiver>();
        if (unit != null && !unit.Dead) unit.Die();
    }
}
