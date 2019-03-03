using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBot : MonoBehaviour, IPauseable, ITimedAction
{
    [Tooltip("Speed of the bot")]
    public float _fSpeed = 5;
    public float _fTurnSpeed = 2;
    [Tooltip("Lifetime in seconds")]
    public float _fLifetime = 5;
    public bool _bMoving;
    public bool _bDebug;
    protected bool _bPaused;
    [SerializeField]
    private LayerMask _lPlatformLayer = 1 << 13;
    private GameObject _Platform;
    private Transform _tPool;
    private Vector3 _PlatformLastPos;
    protected CharacterController _charCon;
    protected OneShotTimer _lifeTimeTimer;

    protected virtual void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _lifeTimeTimer = GetComponent<OneShotTimer>();
        if(!_bDebug)
            _tPool = transform.parent;
    }

    protected virtual void Start()
    {
        AddToPauseCollection();
        if(_bDebug)
            StartMovement();
    }

    protected virtual void FixedUpdate()
    {
        if (!_bPaused)
        {
            Vector3 movement = Vector3.zero;
            movement += Physics.gravity * Time.deltaTime;
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, _charCon.radius * 1.1f, -transform.up, out hit, _charCon.height * 0.6f, _lPlatformLayer)){
                if(hit.collider.gameObject == _Platform){
                    movement += _Platform.transform.position - _PlatformLastPos;
                }
                _Platform = hit.collider.gameObject;
                _PlatformLastPos = _Platform.transform.position;
            } else {
                _Platform = null;
            }
            if (_bMoving)
            {
                movement += transform.forward * _fSpeed * Time.deltaTime;
                if((_charCon.collisionFlags & CollisionFlags.CollidedSides) != 0){
                    _bMoving = false;
                }
            }
            _charCon.Move(movement);
        }
    }

    public virtual void StartMovement()
    {
        _bMoving = true;
        _lifeTimeTimer.SetTimerTarget(this);
        _lifeTimeTimer.StartTimer(_fLifetime);
    }

    public virtual void ResetBot()
    {
        _bMoving = false;
        _lifeTimeTimer.StopTimer();
        gameObject.SetActive(false);
        transform.parent = _tPool;
    }

    public void Pause()
    {
        _bPaused = true;
    }

    public void UnPause()
    {
        _bPaused = false;
    }

    public void AddToPauseCollection()
    {
        GameManager.Instance.AddPauseable(this);
    }

    public void RemoveFromPauseCollection()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.AddPauseable(this);
    }

    protected virtual void OnDestroy()
    {
        RemoveFromPauseCollection();
    }

    public void TimedAction()
    {
        //Used for dying
        ResetBot();
        //Play animations explode
    }

    protected void TurnTowards(GameObject target){
        Vector3 v3TurnDirection = target.transform.position - transform.position;
        v3TurnDirection = v3TurnDirection.normalized;
        v3TurnDirection.y = 0;
        Quaternion qTargetRotation = Quaternion.LookRotation(v3TurnDirection);
        Quaternion qLimitedRotation = Quaternion.Lerp(transform.rotation, qTargetRotation, _fTurnSpeed * Time.deltaTime);
        transform.rotation = qLimitedRotation;
    }

    void OnEnable(){
        if(_bDebug)
            StartMovement();
    }
}
