using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private string attacktrigger = "Attack";
    private CharControlBase charC;

    private void Awake()
    {
        charC = GetComponentInParent<CharControlBase>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
        charC._animator?.SetTrigger(attacktrigger);
    }
}