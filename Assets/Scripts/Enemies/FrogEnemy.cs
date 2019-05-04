using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase
{
    [SerializeField]
    private LayerMask _walkableTerrain = 1 << 12 | 1 << 13 | 1 << 14;
    private float _time = 0;
    public float _circleRadius = 1, _idleTime = 2;
    private bool _stopMoving, _nextStopX, _nextStopZ, _followPlayer, _backToPrevious, _canFollow, _attackStop;
    private PhysicsOneShotTimer _timer;
    private Vector3 _goBackPosition, _playerPosition;
    private Transform _spawnerTransform;
    [HideInInspector]
    public EnemyAttack _attack;

    public float Speed
    {
        set { _speed = value; }
    }

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
        _attack = GetComponentInChildren<EnemyAttack>();
    }

    private void OnEnable()
    {
        _attack.gameObject.SetActive(true);
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
        _time = 0;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= TimedAction;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused) return;
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position + transform.up + transform.forward , 0.5f, transform.TransformDirection(Vector3.down), out hit, 3, _walkableTerrain))
        {
            if (_time > 0.5f)
            {
                _canFollow = false;
                _followPlayer = false;
                _backToPrevious = true;
            }
        }
    }

    protected override Vector2 InternalMovement()
    {
        if (_attackStop) return Vector2.zero;

        float x, z;

        if (!_stopMoving && !_followPlayer && !_backToPrevious)
        {
            
            _time += Time.deltaTime / _circleRadius;

            x = Mathf.Sin(_time);
            z = Mathf.Cos(_time);
        }
        else if (_followPlayer)
        {
            Vector3 goToPlayer = _playerPosition - transform.position;
            x = goToPlayer.x;
            z = goToPlayer.z;
        }
        else if (_backToPrevious)
        {
            Vector3 goBack = _goBackPosition - transform.position;
            x = goBack.x;
            z = goBack.z;
        }
        else 
        {
            x = 0;
            z = 0;
        }

        if(!_backToPrevious && !_followPlayer && _nextStopX && (x > 0.999f || x < -0.999f))
        {
            _nextStopX = false;
            _nextStopZ = true;
            _stopMoving = true;
            _idleTime = Random.Range(1.0f, 3.0f);
            _timer.StartTimer(_idleTime);
 
        }else if (!_backToPrevious && !_followPlayer && _nextStopZ && (z > 0.999f || z < -0.999f))
        {
            _nextStopX = true;
            _nextStopZ = false;
            _stopMoving = true;
            _idleTime = Random.Range(1.0f, 3.0f);
            _timer.StartTimer(_idleTime);   
        }else if(_backToPrevious && x < 0.1f && x > -0.1f && z < 0.1f && z > -0.1f)
        {
            _canFollow= true;
            _backToPrevious = false;
        }       

        Vector2 move = new Vector2(x, z);
        if (!FollowPlayer && !_backToPrevious)
        {
            move = RotateInput(move, _spawnerTransform.eulerAngles.y);
        }
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
            _animator.SetBool("Jump", true);
        }
        _backToPrevious = false;
    }

    public void AggroStay(Transform other)
    {
        if (_canFollow)
        {
            _playerPosition = other.position;
        }
    }

    //when player exits aggro area, go back to previous position
    public void AggroExit()
    {
        _followPlayer = false;
        _backToPrevious = true;
        _animator.SetBool("Jump", false);
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
