using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _defendtrigger = "Defend";
    private PatrolEnemy _scorpion;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _scorpion = GetComponentInParent<PatrolEnemy>();
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    private void OnDisable()
    {
        Dead = false;
    }

    public void Die()
    { 
        if (Dead) return;
        _scorpion._animator.SetTrigger(_defendtrigger);
        _scorpion.StopMoving = true;
        Dead = true;
        _scorpion.SetControllerActive(false);
        //if (_scorpion._deathSound != null) _scorpion._deathSound.PlaySound();
        _scorpion._attack.gameObject.SetActive(false);   
    }
}
