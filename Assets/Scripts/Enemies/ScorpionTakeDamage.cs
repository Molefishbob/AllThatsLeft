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

    public void Die()
    {
        _scorpion._animator?.SetTrigger(_defendtrigger);
        _scorpion.StopMoving = true;
    }
}
