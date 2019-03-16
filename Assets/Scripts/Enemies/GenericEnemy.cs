using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : CharControlBase, IDamageReceiver
{
    protected EnemySpawner _spawner;

    protected override void Awake()
    {
        base.Awake();
        _spawner = GetComponent<EnemySpawner>();
        SetControllerActive(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 10)
        {
            hit.gameObject.GetComponent<ThirdPersonPlayerMovement>().TakeDamage(0);
        }
    }

    protected override Vector3 InternalMovement()
    {
        return Vector3.zero;
    }

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