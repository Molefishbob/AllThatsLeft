﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyMover : CharControlBase, IDamageReceiver
{
    private List<Transform> _transforms;
    private int _targetCounter;
    private bool _goingForward;
    private EnemySpawner _spawner;
    
    public bool _die = false;

    protected override void Start()
    {
        _spawner = GetComponentInParent<EnemySpawner>();
        _targetCounter = 0;
        _goingForward = true;
        Init();
    }

    private void Update()
    {
        if (_die)
            Die();
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 10)
        {
            hit.gameObject.GetComponent<ThirdPersonPlayerMovement>().TakeDamage(0);
        }
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        _spawner.StartTime();
        gameObject.SetActive(false);
    }
}