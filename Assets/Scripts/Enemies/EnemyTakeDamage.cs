using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour, IDamageReceiver
{
    [SerializeField]
    private string _deadBool = "Dead";
    private EnemyMover _frog;

    public bool Dead { get; protected set; }

    private void Awake()
    {
        _frog = GetComponent<EnemyMover>();
    }

    private void OnDisable()
    {
        Dead = false;
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        if (Dead) return;
        Dead = true;
        _frog.SetControllerActive(false);
        if (_frog._deathSound != null) _frog._deathSound.PlaySound();
        _frog._attack.gameObject.SetActive(false);
        _frog._animator.SetBool(_deadBool, true);
    }
}
