using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : CharControlBase, ITimedAction, IDamageReceiver
{
    private float _time = 0;
    public float _circleRadius = 1, _idleTime = 2;
    private bool _stopMoving, _nextStopX, _nextStopZ, _followPlayer, _backToPrevious, _canFollow;
    private OneShotTimer _timer;
    private Vector3 _goBackPosition, _playerPosition;
    public LayerMask _groundLayer;


    protected override void Awake()
    {
        base.Awake();
        _canFollow = true;
        _timer = GetComponent<OneShotTimer>();
        _timer.SetTimerTarget(this);
        _stopMoving = false;
        _nextStopX = true;
        _nextStopZ = false;
        _followPlayer = false;
        _backToPrevious = false;
    }

    private void Update()
    {
        RaycastHit hit;
        if (!Physics.SphereCast(transform.position + transform.forward, 0.5f, transform.TransformDirection(Vector3.down), out hit, 3, _groundLayer))
        {
            _canFollow = false;
            _followPlayer = false;
            _backToPrevious = true;
        }
    }

    protected override Vector3 InternalMovement()
    {
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
        
        return move;
    }

    public void TimedAction()
    {
        _stopMoving = false;
    }

    // When player enters aggro area start chasing, and save the previous position, so frog can return when not chasing anymore
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _followPlayer = true;
            if (!_backToPrevious)
            {
                _goBackPosition = transform.position;
            }
            _backToPrevious = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 10 && _canFollow)
        {
            _playerPosition = other.gameObject.transform.position;
        }
    }

    //when player exits aggro area, go back to previous position
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _followPlayer = false;
            _backToPrevious = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.layer == 10)
        {
            hit.gameObject.GetComponent<ThirdPersonPlayerMovement>().TakeDamage(0);
        }
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {

    }
}