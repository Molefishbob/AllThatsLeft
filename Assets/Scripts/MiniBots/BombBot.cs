using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBot : GenericBot
{
    public float _fExplosionRadius;
    private RepeatingTimer _SurroundCheckTimer;
    private bool _bIsChecking;
    private float _fCheckTime;
    private float _fCheckTimer = 0.25f;
    protected override void Awake(){
        base.Awake();
        _SurroundCheckTimer = GetComponent<RepeatingTimer>();
    }

    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
        if(!_bPaused){
            if(_fCheckTime > _fCheckTimer)
                CheckSurroundings();
            _fCheckTime += Time.deltaTime;
        }
    }

    protected override void StartMovement(){
        base.StartMovement();
        
    }

    void CheckSurroundings(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fExplosionRadius); // Add enemy layer mask or compare tags
        foreach (Collider o in hitColliders){
            if(o.name != "Plane")
                Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
        }
        _fCheckTime = 0;
    }
}
