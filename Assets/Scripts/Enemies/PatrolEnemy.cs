using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : CharControlBase
{
    private List<Transform> _targets = new List<Transform>();
    private int _targetCounter;
    private bool _goingForward;
    private bool _stopMoving;
    private Quaternion _lookAtThis;

    public float Speed
    {
        set { _speed = value; }
    }

    public bool StopMoving
    {
        set { _stopMoving = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        _targetCounter = 0;
        _goingForward = true;
    }

    private void OnDisable()
    {
        _targetCounter = 0;
        _goingForward = true;
        _stopMoving = false;
    }

    protected override void Start()
    {

        SetControllerActive(true);
    }

    public List<Transform> Targets
    {
        set { _targets = value; }
        get { return _targets; }
    }

    private void Update()
    {
        Quaternion oldRotation = transform.rotation;
        if (_stopMoving)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookAtThis, _turningSpeed * Time.deltaTime);
        }
        if(oldRotation == transform.rotation)
        {
            _stopMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 23)
        {
            _stopMoving = true;
            
            if (_goingForward)
            {
                _targetCounter++;
            }
            else if (!_goingForward && _targetCounter > 0)
            {
                _targetCounter--;
            }
            else if (!_goingForward && _targetCounter == 0)
            {
                _targetCounter++;
                _goingForward = true;
            }

            if (_targetCounter > _targets.Count - 1)
            {
                _goingForward = false;
                _targetCounter = _targets.Count - 2;

            }
            else if (_targetCounter == 0)
            {
                _goingForward = true;
            }

            _lookAtThis = Quaternion.LookRotation(_targets[_targetCounter].position - transform.position);
        }
    }

    protected override Vector3 InternalMovement()
    {
        if (_stopMoving) return Vector3.zero;

        Vector3 moveDirection = _targets[_targetCounter].position - transform.position;

        moveDirection.y = 0;

        return moveDirection;
    }

    protected override void OutOfBounds()
    {
        transform.parent.gameObject.SetActive(false);
        transform.parent.localPosition = Vector3.zero;
    }
}