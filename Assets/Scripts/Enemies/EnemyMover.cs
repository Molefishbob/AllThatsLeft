using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : CharControlBase
{
    public Vector3 _target;
    private bool _stopMoving = false;
    [HideInInspector]
    public EnemyAttack _attack;

    protected override void Awake()
    {
        base.Awake();
        _attack = GetComponentInChildren<EnemyAttack>();
    }

    public bool StopMoving
    {
        set { _stopMoving = value; }
    }

    public float Speed
    {
        set { _speed = value; }
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    private void OnEnable()
    {
        _attack.gameObject.SetActive(true);
    }

    protected override Vector3 InternalMovement()
    {

        if (_stopMoving) return Vector3.zero;

        Vector3 newDir = _target - transform.position;
        newDir.Normalize();
        return newDir;
    }

    protected override void OutOfBounds()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }
}
