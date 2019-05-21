using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private string attacktrigger = "Attack";
    private EnemyMover _frog;
    private PatrolEnemy _scorpion;

    private void Awake()
    {
        if (transform.parent.tag == "Frog")
        {
            _frog = GetComponentInParent<EnemyMover>();
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
            _frog._animator.SetTrigger(attacktrigger);
            if (_frog._attackSound != null && _frog.gameObject.activeInHierarchy) _frog._attackSound.PlaySound();
            _frog.StopMoving = true;
        }
        if(_scorpion != null)
        {
            _scorpion._animator.SetTrigger(attacktrigger);
            if (_scorpion._attackSound != null && _scorpion.gameObject.activeInHierarchy) _scorpion._attackSound.PlaySound();
            _scorpion.StopMoving = true;
        }
    }
}
