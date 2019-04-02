using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : CharControlBase
{
    private List<Transform> _targets = new List<Transform>();
    private int _targetCounter;
    private bool _goingForward;

    protected override void Awake()
    {
        base.Awake();
        _targetCounter = 0;
        _goingForward = true;
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

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 23)
        {
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
        }
    }

    protected override Vector3 InternalMovement()
    {
        //Debug.Log(_targetCounter + " jeejee " + _transforms.Count);

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