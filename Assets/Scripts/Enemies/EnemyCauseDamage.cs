using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCauseDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
    }
}
