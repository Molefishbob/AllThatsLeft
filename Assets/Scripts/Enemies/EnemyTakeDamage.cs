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
        if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
