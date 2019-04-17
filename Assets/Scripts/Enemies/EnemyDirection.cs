using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirection : MonoBehaviour
{
    private float _aggroRadius;
    [Tooltip("How much smaller the patrolradius is compared to the aggroradius trigger")]
    public float _patrolRadiusDecrease;
    public float _speed = 4;
    private float _patrolRadius;
    private float _idleTime;
    [SerializeField]
    private float _minIdleTime = 1.0f;
    [SerializeField]
    private float _maxIdleTime = 3.0f;
    public EnemyMover _enemy;
    [HideInInspector]
    public List<Transform> _aggroTargets;
    private SphereCollider _aggroArea;
    private Vector3 _moveTarget;
    private PhysicsOneShotTimer _timer;

    private void Awake()
    {
        _timer = GetComponent<PhysicsOneShotTimer>();
        _aggroArea = GetComponent<SphereCollider>();
        _aggroRadius = _aggroArea.radius;
        _patrolRadius = _aggroRadius - _patrolRadiusDecrease;
        _aggroTargets = new List<Transform>(4);
    }

    private void Start()
    {
        _timer.OnTimerCompleted += TimedAction;
    }

    private void OnEnable()
    {
        _enemy.Speed = _speed;
    }

    private void OnDisable()
    {
        _aggroTargets.Clear();
        _timer.StopTimer();
        _enemy.Speed = _speed;
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.OnTimerCompleted -= TimedAction;
        }
    }

    private void FixedUpdate()
    {
        if (_aggroTargets == null || _aggroTargets.Count == 0)
        {
            _enemy.Speed = _speed;
            float dist = Vector3.Distance(_moveTarget, _enemy.transform.position);

            if (dist < 1.0f && !_timer.IsRunning)
            {
                _enemy.StopMoving = true;
                _idleTime = Random.Range(_minIdleTime, _maxIdleTime);
                _timer.StartTimer(_idleTime);
            }
        }
        else
        {
            _enemy.Speed = _speed * 2;
            _moveTarget = _aggroTargets[0].position;
           
            _enemy.SetTarget(_moveTarget);
        }
    }

    public void SetRandomTarget()
    {
        Quaternion angle = Quaternion.Euler(0, Random.Range(-180.0f, 180.0f), 0);
        
        float distance = Random.Range(0.0f, _patrolRadius);

        _moveTarget = transform.TransformPoint(angle * Vector3.forward  * distance);
        
        if (_enemy != null)
        {
            _enemy.SetTarget(_moveTarget);
        }
    }    

    private void TimedAction()
    {
        SetRandomTarget();
        _enemy.StopMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _enemy.StopMoving = false;
        _enemy._animator?.SetBool("Jump", true);
        _aggroTargets.Add(other.transform);
    }

    /*private void OnTriggerStay(Collider other)
    {
        
    }*/

    private void OnTriggerExit(Collider other)
    {
        _enemy.Speed = _speed;
        _enemy._animator?.SetBool("Jump", false);
        _aggroTargets.Remove(other.transform);
    }

    private void OnDrawGizmosSelected()
    {
        _aggroArea = GetComponent<SphereCollider>();
        _aggroRadius = _aggroArea.radius;
        _patrolRadius = _aggroRadius - _patrolRadiusDecrease;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
    }
}