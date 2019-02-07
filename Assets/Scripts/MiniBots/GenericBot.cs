using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBot : MonoBehaviour, IPauseable, ITimedAction
{
    [Tooltip("Speed of the bot")]
    public float _fSpeed = 5;
    [Tooltip("Lifetime in seconds")]
    public float _fLifetime = 5;
    [SerializeField]
    private bool _bMoving;
    protected bool _bPaused;
    private CharacterController _charCon;
    private OneShotTimer _lifeTimeTimer;

    protected virtual void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _lifeTimeTimer = GetComponent<OneShotTimer>();
    }

    protected virtual void Start()
    {
        AddToPauseCollection();
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
        _lifeTimeTimer.StartTimer(_fLifetime, this);
    }

    protected virtual void ResetBot(){/*Waiting for pooling*/}

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
        GameManager.Instance.Pauseables.Add(this);
    }

    public void RemoveFromPauseCollection()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.Pauseables.Remove(this);
    }

    protected virtual void OnDestroy()
    {
        RemoveFromPauseCollection();
    }

    public void TimedAction()
    {
        gameObject.SetActive(false);
        //Play animations explode
    }
}
