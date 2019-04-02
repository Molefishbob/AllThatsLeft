using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private string attacktrigger = "Attack";
    private CharControlBase charC;
    private FrogEnemy _frog;
    private PatrolEnemy _scorpion;

    private void Awake()
    {
        charC = GetComponentInParent<CharControlBase>();
        if (transform.parent.tag == "Frog")
        {
            _frog = GetComponentInParent<FrogEnemy>();
        }
        else if (transform.parent.tag == "Scorpion")
        {
            _scorpion = GetComponentInParent<PatrolEnemy>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageReceiver>().Die();
        charC._animator?.SetTrigger(attacktrigger);       
        if(_frog != null)
        {
            _frog.StopMoving();
            _frog.BackToPrevious = true;
            _frog.FollowPlayer = false;
        }
        if(_scorpion != null)
        {
            _scorpion.StopMoving = true;
        }
    }
}