using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : CharControlBase
{
    public Vector3 _target;

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    protected override Vector3 InternalMovement()
    {
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
