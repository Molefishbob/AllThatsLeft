using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBot : GenericBot
{
    [Tooltip("Units used is the unity Grid 1 square = 1 unit in radius")]
    public float _fExplosionRadius;
    public float _fTurnSpeed = 2;
    private bool _bIsChecking;
    private bool _bIsExploding;
    private float _fCheckTime;
    private GameObject _goClosestObject = null;
    private float _fCheckTimer = 0.25f;
    [Tooltip("The delay for explosion after noticing something")]
    public float _fExplodeTimer = 1f;
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
            if(_fCheckTime > _fCheckTimer && _bIsChecking){
                CheckSurroundings();
            } else if (_bIsExploding && _fCheckTime > _fExplodeTimer){
                Explode();
            }
            if((_charCon.collisionFlags & CollisionFlags.CollidedSides) != 0 && !_bIsChecking)
                Explode();

            if(_goClosestObject != null){
                Vector3 v3TurnDirection = _goClosestObject.transform.position - transform.position;
                v3TurnDirection = v3TurnDirection.normalized;
                v3TurnDirection.y = 0;
                Quaternion qTargetRotation = Quaternion.LookRotation(v3TurnDirection);
                Quaternion qLimitedRotation = Quaternion.Lerp(transform.rotation, qTargetRotation, _fTurnSpeed * Time.deltaTime);
                transform.rotation = qLimitedRotation;
            }
            
            _fCheckTime += Time.deltaTime;
        }
    }

    protected override void StartMovement(){
        base.StartMovement();
        _bIsChecking = true;
    }

    void CheckSurroundings(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fExplosionRadius, _lUnignoredLayers);
        float shortestDistance = float.MaxValue;
        foreach (Collider o in hitColliders){
            Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
            if(shortestDistance > Vector3.Distance(o.transform.position, transform.position) && o.gameObject.layer == 15){
                shortestDistance = Vector3.Distance(o.transform.position, transform.position);
                _goClosestObject = o.gameObject;
            }
            _bIsExploding = true;
            _bIsChecking = false;
        }
        if(_goClosestObject != null){
            if(_goClosestObject.layer == 15){
                Debug.Log("Turning towards CLOSEST BOMBABLE object. Distance : " + shortestDistance);
            }
        }
        _fCheckTime = 0;
    }

    private void Explode(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fExplosionRadius, _lUnignoredLayers);
        foreach (Collider o in hitColliders){
            Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
            o.gameObject.SetActive(false); // Do something smarter here Kill enemies and push walls
        }
        Debug.Log("Exploding " + hitColliders.Length + " objects");
        _bIsExploding = false;
        _goClosestObject = null;
        // Particles!
        ResetBot();
    }

    void OnEnable(){
        if(_bDebug)
            StartMovement();
    }
}
