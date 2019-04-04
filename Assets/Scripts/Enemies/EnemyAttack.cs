using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private string attacktrigger = "Attack";
    private FrogEnemy _frog;
    private PatrolEnemy _scorpion;

    private void Awake()
    {
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
              
        if(_frog != null)
        {
            _frog._animator?.SetTrigger(attacktrigger);
            _frog.StopMoving();
            _frog.BackToPrevious = true;
            _frog.FollowPlayer = false;
        }
        if(_scorpion != null)
        {
            _scorpion._animator?.SetTrigger(attacktrigger);
            _scorpion.StopMoving = true;
        }
    }
}