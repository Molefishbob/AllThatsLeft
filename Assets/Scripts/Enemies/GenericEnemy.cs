using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : CharControlBase, IDamageReceiver
{
    protected EnemySpawner _spawner;
    private Transform _pool;

    protected override void Awake()
    {
        base.Awake();
        _spawner = GetComponent<EnemySpawner>();
        _pool = transform.parent;
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
        _spawner.StartTime();
        transform.parent = _pool;
        transform.position = Vector3.zero;
        ResetGravity();
        gameObject.SetActive(false);
    }
}
