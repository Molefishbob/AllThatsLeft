using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase
{
    private float _time = 0;
    public float _circleRadius = 1, _idleTime = 2;
    private bool _stopMoving, _nextStopX, _nextStopZ, _followPlayer, _backToPrevious, _canFollow, _attackStop;
    private PhysicsOneShotTimer _timer;
    private Vector3 _goBackPosition, _playerPosition;
    public LayerMask _groundLayer;
    private Transform _spawnerTransform;

    protected override void Awake()
    {
        base.Awake();
        _canFollow = true;
        _timer = GetComponent<PhysicsOneShotTimer>();
        _stopMoving = false;
        _nextStopX = true;
        _nextStopZ = false;
        _followPlayer = false;
        _backToPrevious = false;
    }

    private void OnDisable()
    {
        _stopMoving = false;
        _nextStopX = true;
        _nextStopZ = false;
        _followPlayer = false;
        _backToPrevious = false;
        _canFollow = true;
        _attackStop = false;
    }

    public bool BackToPrevious
    {
        set {_backToPrevious = value;}
        get { return _backToPrevious; }
    }
    public bool FollowPlayer
    {
        set { _followPlayer = value; }
        get { return _followPlayer; }
    }

    protected override void Start()
    {
        base.Start();
        SetControllerActive(true);
        _timer.OnTimerCompleted += TimedAction;
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= TimedAction;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position + transform.up + transform.forward , 0.5f, transform.TransformDirection(Vector3.down), out hit, 3, _groundLayer))
        {
            if (_time > 0.5f)
            {
                _canFollow = false;
                _followPlayer = false;
                _backToPrevious = true;
            }
        }
    }

    protected override Vector3 InternalMovement()
    {
        if (_attackStop) return Vector3.zero;

        float x, y, z;

        if (!_stopMoving && !_followPlayer && !_backToPrevious)
        {
            
            _time += Time.deltaTime / _circleRadius;

            x = Mathf.Sin(_time);
            y = 0;
            z = Mathf.Cos(_time);
        }
        else if (_followPlayer)
        {
            Vector3 goToPlayer = _playerPosition - transform.position;
            x = goToPlayer.x;
            y = 0;
            z = goToPlayer.z;
        }
        else if (_backToPrevious)
        {
            Vector3 goBack = _goBackPosition - transform.position;
            x = goBack.x;
            y = 0;
            z = goBack.z;
        }
        else 
        {
            x = 0;
            y = 0;
            z = 0;
        }

        if(!_backToPrevious && !_followPlayer && _nextStopX && (x > 0.999f || x < -0.999f))
        {
            _nextStopX = false;
            _nextStopZ = true;
            _stopMoving = true;
            _timer.StartTimer(_idleTime);
 
        }else if (!_backToPrevious && !_followPlayer && _nextStopZ && (z > 0.999f || z < -0.999f))
        {
            _nextStopX = true;
            _nextStopZ = false;
            _stopMoving = true;
            _timer.StartTimer(_idleTime);   
        }else if(_backToPrevious && x < 0.1f && x > -0.1f && z < 0.1f && z > -0.1f)
        {
            _canFollow= true;
            _backToPrevious = false;
        }       

        Vector3 move = new Vector3(x,y,z);
        move = _spawnerTransform.TransformDirection(move);
        return move;
    }

    private void TimedAction()
    {
        _stopMoving = false;
    }

    // When player enters aggro area start chasing, and save the previous position, so frog can return when not chasing anymore
    public void AggroEnter()
    {
        _followPlayer = true;
        if (!_backToPrevious)
        {
            _goBackPosition = transform.position;
        }
        _backToPrevious = false;
    }

    public void AggroStay(Transform other)
    {
        if (_canFollow)
        {
            _playerPosition = other.gameObject.transform.position;
        }
    }

    //when player exits aggro area, go back to previous position
    public void AggroExit()
    {
        _followPlayer = false;
        _backToPrevious = true;
    }
    
    protected override void OutOfBounds()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

    public void StartMoving()
    {
        _attackStop = false;
    }

    public void StopMoving()
    {
        _attackStop = true;
    }

    public void SetSpawnerTransform(Transform trans)
    {
        _spawnerTransform = trans;        
    }
}