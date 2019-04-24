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
    private PhysicsOneShotTimer _targetTimer;
    private PhysicsOneShotTimer _burpTimer;
    [SerializeField]
    private float _minBurpWait = 5.0f;
    [SerializeField]
    private float _maxBurpWait = 15.0f;

    private void Awake()
    {
        _targetTimer = GetComponent<PhysicsOneShotTimer>();
        _burpTimer = GetComponent<PhysicsOneShotTimer>();
        _aggroArea = GetComponent<SphereCollider>();
        _aggroRadius = _aggroArea.radius;
        _patrolRadius = _aggroRadius - _patrolRadiusDecrease;
        _aggroTargets = new List<Transform>(4);
    }

    private void Start()
    {
        _targetTimer.OnTimerCompleted += TimedAction;
        _burpTimer.OnTimerCompleted += TimedBurp;
    }

    private void OnEnable()
    {
        _enemy.Speed = _speed;
    }

    private void OnDisable()
    {
        _aggroTargets.Clear();
        _targetTimer.StopTimer();
        _burpTimer.StopTimer();
        _enemy.Speed = _speed;
    }

    private void OnDestroy()
    {
        if (_targetTimer != null)
        {
            _targetTimer.OnTimerCompleted -= TimedAction;
        }

        if (_burpTimer != null)
        {
            _burpTimer.OnTimerCompleted -= TimedBurp;
        }
    }

    private void FixedUpdate()
    {
        RemoveDeadTargets();
        
        if (_aggroTargets == null || _aggroTargets.Count == 0)
        {
            /*if (!_burpTimer.IsRunning)
            {
                _burpTimer.StartTimer(Random.Range(_minBurpWait, _maxBurpWait));
            }*/
            _enemy.Speed = _speed;
            float dist = Vector3.Distance(_moveTarget, _enemy.transform.position);

            if (dist < 1.0f && !_targetTimer.IsRunning)
            {
                _enemy.StopMoving = true;
                _idleTime = Random.Range(_minIdleTime, _maxIdleTime);
                _targetTimer.StartTimer(_idleTime);
                _enemy.DirTimerRunning = true;
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
        _enemy.DirTimerRunning = false;
        SetRandomTarget();
        _enemy.StopMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _enemy._animator?.SetBool("Jump", true);
        _aggroTargets.Add(other.transform);
        if (_aggroTargets.Count <= 1)
        {
            Alert();
        }
    }

    private void RemoveDeadTargets()
    {
        int targetCount = _aggroTargets.Count;

        for(int i = 0; i < _aggroTargets.Count; i++)
        {
            if (_aggroTargets[i].GetComponent<IDamageReceiver>().Dead || !_aggroTargets[i].gameObject.activeSelf)
            {
                _aggroTargets.Remove(_aggroTargets[i].transform);
            }
        }

        if (_aggroTargets.Count == 0 && targetCount > 0)
        {
            _enemy._animator?.SetBool("Jump", false);
            SetRandomTarget();
        }
    }

    public void RemoveTarget(Transform other)
    {
        bool isSameTarget = _aggroTargets[0] == other;

        _aggroTargets.Remove(other);

        if (_aggroTargets.Count == 0)
        {
            _enemy._animator?.SetBool("Jump", false);
            SetRandomTarget();
        }
        else if(isSameTarget)
        {
            Alert();
        }
    }

    private void Alert()
    {
        _enemy.StopMoving = true;
        _enemy._animator?.SetTrigger("Alert");
        if (_enemy._alertSound != null) _enemy._alertSound.PlaySound();
    }

    public void CheckTargets()
    {
        if (_aggroTargets.Count <= 1)
        {
            Alert();
        }
        else
        {
            _enemy.StopMoving = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveTarget(other.transform);
    }

    private void OnDrawGizmosSelected()
    {
        _aggroArea = GetComponent<SphereCollider>();
        _aggroRadius = _aggroArea.radius;
        _patrolRadius = _aggroRadius - _patrolRadiusDecrease;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
    }

    private void TimedBurp()
    {
        if (_enemy._burpsfx != null) _enemy._burpsfx.PlaySound(true, Random.Range(1, 5));
    }
}