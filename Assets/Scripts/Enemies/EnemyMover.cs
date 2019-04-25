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
    public SingleSFXSound _alertSound;
    [HideInInspector]
    public EnemyDirection _eDirect;
    public RandomSFXSound _burpSound = null;
    public RandomSFXSound _deathSound = null;
    public SingleSFXSound _attackSound;

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

    protected override Vector2 InternalMovement()
    {
        if (_stopMoving) return Vector2.zero;

        Vector3 newDir = _target - transform.position;
        Vector2 move = new Vector2(newDir.x, newDir.z);
        move.Normalize();
        return move;
    }

    protected override void OutOfBounds()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }
}
