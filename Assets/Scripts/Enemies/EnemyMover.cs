using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : CharControlBase
{
    public Vector3 _target;
    private Vector3 _empty;
    private bool _stopMoving = false;
    [HideInInspector]
    public EnemyAttack _attack;
    private bool _dirTimerRunning;

    public bool DirTimerRunning
    {
        set { _dirTimerRunning = value; }
        get { return _dirTimerRunning; }
    }

    protected override void Awake()
    {
        base.Awake();
        _attack = GetComponentInChildren<EnemyAttack>();
    }

    public bool StopMoving
    {
        set { _stopMoving = value; }
        get { return _stopMoving; }
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
        _stopMoving = false;
        _attack.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _stopMoving = false;
        _attack.gameObject.SetActive(false);
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
