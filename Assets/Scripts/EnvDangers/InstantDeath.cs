using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    [SerializeField]
    private SingleSFXSound _zapSound = null;
    private void OnTriggerEnter(Collider other)
    {
        IDamageReceiver unit = other.GetComponent<IDamageReceiver>();
        if (unit != null && !unit.Dead)
        {
            _zapSound.PlaySound();
            unit.Die();
        }
    }
}
