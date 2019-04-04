using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour, IDamageReceiver
{

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        
        gameObject.SetActive(false);
        
    }
}
