using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirection : MonoBehaviour
{
    private float _aggroRadius;
    [Tooltip("How much smaller the patrolradius is compared to the aggroradius trigger")]
    public float _patrolRadiusDecrease;
    private float _patrolRadius;
    [SerializeField]
    private float _idleTime = 1;
    public EnemyMover _enemy;
    [HideInInspector]
    public List<Transform> _aggroTargets;
    private SphereCollider _aggroArea;
    private Vector3 _moveTarget;
    private PhysicsOneShotTimer _timer;
    private bool _stopMoving;

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
        SetRandomTarget();
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
            float dist = Vector3.Distance(_moveTarget, _enemy.transform.position);
            
            if (dist < 1.0f)
            {
                
                _stopMoving = true;
                _idleTime = Random.Range(1.0f, 3.0f);
                _timer.StartTimer(_idleTime);
            }
        }
        else
        {
            _moveTarget = _aggroTargets[0].position;
           
            _enemy.SetTarget(_moveTarget);
        }
    }

    private void SetRandomTarget()
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
        _stopMoving = false;  
    }

    private void OnTriggerEnter(Collider other)
    {
        _aggroTargets.Add(other.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        _aggroTargets.Remove(other.transform);
    }

    private void OnDrawGizmosSelected()
    {
        _aggroArea = GetComponent<SphereCollider>();
        _aggroRadius = _aggroArea.radius;
        _patrolRadius = _aggroRadius - _patrolRadiusDecrease;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
    }
}