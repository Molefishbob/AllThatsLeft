using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private string attacktrigger = "Attack";
    private CharControlBase charC;
    private FrogEnemy _frog;

    private void Awake()
    {
        charC = GetComponentInParent<CharControlBase>();   
        if(transform.parent.tag == "Frog"){
            _frog = GetComponentInParent<FrogEnemy>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
        charC._animator?.SetTrigger(attacktrigger);
        if(_frog != null)
        {
            _frog.BackToPrevious = true;
            _frog.FollowPlayer = false;
        }
    }
}