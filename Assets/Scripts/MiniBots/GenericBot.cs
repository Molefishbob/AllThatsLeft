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
    [SerializeField]
    private bool _bMoving;
    public bool _bDebug;
    protected bool _bPaused;
    protected CharacterController _charCon;
    private OneShotTimer _lifeTimeTimer;

    protected virtual void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _lifeTimeTimer = GetComponent<OneShotTimer>();
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
            if (_bMoving)
            {
                Vector3 movement = transform.forward * _fSpeed * Time.deltaTime;
                movement += Physics.gravity * Time.deltaTime;
                _charCon.Move(movement);
                if((_charCon.collisionFlags & CollisionFlags.CollidedSides) != 0){
                    _bMoving = false;
                }
            }
        }
    }

    protected virtual void StartMovement()
    {
        _bMoving = true;
        _lifeTimeTimer.SetTimerTarget(this);
        _lifeTimeTimer.StartTimer(_fLifetime);
    }

    protected virtual void ResetBot()
    {
        _bMoving = false;
        _lifeTimeTimer.StopTimer();
        gameObject.SetActive(false);
        // TODO: Return to pool
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
}
