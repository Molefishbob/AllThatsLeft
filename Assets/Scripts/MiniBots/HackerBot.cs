using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackerBot : GenericBot
{
    public float _fDetectRadius = 3f;
    private float _fStopRange = 2f;
    public LayerMask _lHackableLayer;
    private float _fCheckTimer = 0.25f;
    private float _fCheckTime = 0.25f;
    private bool _bIsChecking = false;
    private GameObject _goClosestObject = null;
    
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(!_bPaused){
            if(_fCheckTime >= _fCheckTimer && _bIsChecking){
                CheckSurroundings();
            }
            _fCheckTime += Time.deltaTime;

            if(_goClosestObject != null){
                TurnTowards(_goClosestObject);
                if((transform.position - _goClosestObject.transform.position).magnitude < _fStopRange){
                    _bMoving = false;
                    _lifeTimeTimer.StopTimer();
                    // say to the console im here maybe
                }
            }
        }
    }

    protected override void StartMovement(){
        base.StartMovement();
        _bIsChecking = true;
    }

    void CheckSurroundings(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fDetectRadius, _lHackableLayer);
        float shortestDistance = float.MaxValue;
        foreach (Collider o in hitColliders){
            Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
            int tmp = 1 << o.gameObject.layer;
            if(shortestDistance > Vector3.Distance(o.transform.position, transform.position) && tmp == _lHackableLayer){
                shortestDistance = Vector3.Distance(o.transform.position, transform.position);
                _goClosestObject = o.gameObject;
            }
            _bIsChecking = false;
        }
        _fCheckTime = 0;
    }

    public override void ResetBot(){
        base.ResetBot();
        _goClosestObject = null;
    }
}
