using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : CharControlBase
{
    private int _targetCounter = 0;
    private bool _goingForward = true;
    private bool _stopMoving = false;
    private bool _turningStop;
    private Quaternion _lookAtThis;
    [HideInInspector]
    public EnemyAttack _attack;
    private string _defaultAnimState = "Default";
    [HideInInspector]
    public ScorpionSpawner _spawner;
    [HideInInspector]
    public bool _dead = false;
    public ParticleSystem _teleportEffect;
    public SingleSFXSound _deathSound;
    public SingleSFXSound _walkSound;
    public SingleSFXSound _attackSound;
    public SingleSFXSound _dissolveSound;

    public bool StopMoving
    {
        set { _stopMoving = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        _attack = GetComponentInChildren<EnemyAttack>(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _attack.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _targetCounter = 0;
        _goingForward = true;
        _stopMoving = false;
        _dead = false;
        _animator.SetTrigger(_defaultAnimState);
        _attack.gameObject.SetActive(false);
    }

    /*protected override void Start()
    {
        //SetControllerActive(true);
        
        var main = _teleportEffect.main;
        main.duration = _spawner._teleportTime;
    }*/

    [HideInInspector]
    public List<Transform> Targets;

    private void Update()
    {
        if (_turningStop)
        {
            Quaternion oldRotation = transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookAtThis, _turningSpeed * Time.deltaTime);
            if (Quaternion.Angle(oldRotation, transform.rotation) <= 2)
            {
                _turningStop = false;
            }
        }
        else if (Vector3.Distance(transform.position, Targets[_targetCounter].position) <= 0.1f)
        {
            ChangeTarget();
        }
    }

    private void ChangeTarget()
    {
        _turningStop = true;

        if (_targetCounter >= Targets.Count - 1)
        {
            _goingForward = false;
        }
        else if (_targetCounter <= 0)
        {
            _goingForward = true;
        }
        _targetCounter += _goingForward ? 1 : -1;

        _lookAtThis = Quaternion.LookRotation(Targets[_targetCounter].position - transform.position, Vector3.up);
    }

    protected override Vector2 InternalMovement()
    {
        if (_stopMoving || _turningStop) return Vector2.zero;

        Vector3 moveDirection = Targets[_targetCounter].position - transform.position;
        Vector2 move = new Vector2(moveDirection.x, moveDirection.z);
        move.Normalize();
        return move;
    }

    protected override void OutOfBounds()
    {
        transform.parent.gameObject.SetActive(false);
        transform.parent.localPosition = Vector3.zero;
    }
}
