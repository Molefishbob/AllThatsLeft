using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : CharControlBase
{
    

    protected override Vector3 InternalMovement()
    {
        return Vector3.zero;
    }

    protected override void OutOfBounds()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }
}
