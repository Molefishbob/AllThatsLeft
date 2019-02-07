using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBot : GenericBot
{
    [Tooltip("The RADIUS of explosion")]
    public float _fExplosionRadius;
    private bool _bIsChecking;
    private float _fCheckTime;
    private float _fCheckTimer = 0.25f;
    [Tooltip("Put in enemy and other interactables here if needed")]
    public LayerMask _lUnignoredLayers;
    protected override void Awake(){
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(!_bPaused){
            if(_fCheckTime > _fCheckTimer && _bIsChecking)
                CheckSurroundings();
            _fCheckTime += Time.deltaTime;
        }
    }

    protected override void StartMovement(){
        base.StartMovement();
        _bIsChecking = true;
    }

    void CheckSurroundings(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fExplosionRadius, _lUnignoredLayers); // Add enemy layer mask or compare tags
        foreach (Collider o in hitColliders){
            Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
            o.gameObject.SetActive(false);
        }
        _fCheckTime = 0;
    }
}
