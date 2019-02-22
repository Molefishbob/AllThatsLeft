using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBackAndForthEnemy : CharControlBase
{
    private List<Transform> _transforms;
    private int _targetCounter;
    private bool _goingForward;

    //TODO: make everything better

    protected override void Start()
    {
        _targetCounter = 0;
        _goingForward = false;
        Init();
    }

    private void Update()
    {
        if (transform.position.x - _transforms[_targetCounter].position.x < 0.2f && transform.position.z - _transforms[_targetCounter].position.z < 0.2f
            && transform.position.x - _transforms[_targetCounter].position.x > -0.2f && transform.position.z - _transforms[_targetCounter].position.z > -0.2f)
        {
            if (_goingForward)
            {
                _targetCounter++;
            }
            else
            {
                _targetCounter--;
            }

            if (_targetCounter > _transforms.Count - 1)
            {
                _goingForward = false;
                _targetCounter = _transforms.Count - 2;

            }else if (_targetCounter < 0)
            {
                _goingForward = true;
                _targetCounter = 0;
            }  
        }
        
    }

    public virtual void Init()
    {
        _transforms = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                _transforms.Add(child);
            }
        }
    }

    protected override Vector3 InternalMovement()
    {

        Vector3 moveDirection = _transforms[_targetCounter].position - transform.position;

        moveDirection.y = 0;

        return moveDirection;
    }
}
