using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBot : GenericBot
{
    [Tooltip("Units used is the unity Grid 1 square = 1 unit in radius")]
    public float _fExplosionRadius;
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

    protected override void FixedUpdateAdditions(){
        if(_fCheckTime > _fCheckTimer && _bIsChecking){
            CheckSurroundings();
        } else if (_bIsExploding && _fCheckTime > _fExplodeTimer){
            Explode();
        }
        if((_controller.collisionFlags & CollisionFlags.CollidedSides) != 0 && !_bIsChecking)
            Explode();

        if(_goClosestObject != null){
            TurnTowards(_goClosestObject);
        }
        
        _fCheckTime += Time.deltaTime;
    }

    public override void StartMovement(){
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
        _fCheckTime = 0;
    }

    private void Explode(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fExplosionRadius, _lUnignoredLayers);
        foreach (Collider o in hitColliders){
            Debug.DrawLine(transform.position, o.transform.position, Color.red, 5f);
            o.gameObject.SetActive(false); // Do something smarter here Kill enemies and push walls
        }
        _bIsExploding = false;
        _goClosestObject = null;
        // Particles!
        ResetBot();
    }
}
