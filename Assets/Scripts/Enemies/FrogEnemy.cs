using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase
{
    private float _time;

    protected override Vector3 InternalMovement()
    {
        //just testing things for now
        _time += Time.deltaTime * 5;
        
        float x = Mathf.Sin(_time) * 10;
        float y = 0;
        float z = Mathf.Cos(_time) * 5;
        
        Vector3 move = new Vector3(x,y,z);
        Debug.Log(move);
        return move;
    } 
}
