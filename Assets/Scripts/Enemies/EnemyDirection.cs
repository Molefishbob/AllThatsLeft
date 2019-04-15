using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirection : MonoBehaviour
{
    private float _aggroRadius;
    [Tooltip("How much smaller the patrolradius is compared to the aggroradius trigger")]
    public float _patrolRadiusDecrease;
    private float _patrolRadius;
    private Quaternion _angle;
    private float _distance;
    [SerializeField]
    private float _idleTime = 1;
    [SerializeField]
    private EnemyMover _prefab;
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
        SetRandomTarget();
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
        if (_aggroTargets == null || _aggroTargets.Count == 0)
        {
            float dist = Mathf.Abs(Vector3.Distance(_moveTarget, _prefab.transform.position));

            if (dist < 1.0f)
            {
                _stopMoving = true;
                _idleTime = Random.Range(1.0f, 3.0f);
                _timer.StartTimer(_idleTime);
            }
        }
        else
        {
            _moveTarget = _aggroTargets[0].position - _prefab.transform.position;
        }
    }

    private void SetRandomTarget()
    {
        _angle = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        _distance = Random.Range(1.0f, _patrolRadius);

        _moveTarget = transform.TransformDirection(_angle * Vector3.forward * _distance);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _patrolRadius);
    }
}