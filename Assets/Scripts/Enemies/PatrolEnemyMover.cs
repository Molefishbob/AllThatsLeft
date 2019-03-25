using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyMover : CharControlBase
{
    private List<Transform> _transforms = new List<Transform>();
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
        Initialize();
    }

    private void Initialize()
    {
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                _transforms.Add(child);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == 23)
        {
            if (_goingForward)
            {
                _targetCounter++;
            }
            else if(!_goingForward && _targetCounter > 0)
            {
                _targetCounter--;
            } else if (!_goingForward && _targetCounter == 0)
            {
                _targetCounter++;
                _goingForward = true;
            }

            if (_targetCounter > _transforms.Count - 1)
            {
                _goingForward = false;
                _targetCounter = _transforms.Count - 2;

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

        Vector3 moveDirection = _transforms[_targetCounter].position - transform.position;

        moveDirection.y = 0;

        return moveDirection;
    }

    protected override void OutOfBounds()
    {
        transform.parent.gameObject.SetActive(false);
        transform.parent.localPosition = Vector3.zero;
    }
}