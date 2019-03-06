using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMover : MonoBehaviour, ITimedAction, IButtonInteraction
{

    [Tooltip("The amount of time it takes to go the whole length")]
    public float _duration;
    protected float _eventTime;
    protected List<Transform> _transform;
    protected float _fracTime;
    protected int _amountOfTransforms;
    protected int _currentObjectNum = 0;
    protected int _nextObjectNum = 1;
    protected float _length;
    protected float _ogStartTime;
    protected int _trackRecord = 0;
    protected RepeatingTimer _timer;
    [SerializeField]
    protected bool _activated = true;

    protected virtual void Awake()
    {
        _timer = GetComponent<RepeatingTimer>();
        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent) {
            if (child != transform) {
                _transform.Add(child);
            }
        }
        
        _amountOfTransforms = _transform.Count - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _transform[0].position;
        if (_activated)
        {
            Init();
        }
    }

    /// <summary>
    /// Initializes the script.
    /// 
    /// Gets all the checkpoints and counts the complete length of the trip.
    /// The length is later used to calculate the speed between objects.
    /// </summary>
    public virtual void Init()
    {
        _length = 0;

        _timer.SetTimerTarget(this);
        _timer.StartTimer(_duration);

        for (int a = 0; a < _transform.Count-1; a++)
        {
            _length += (_transform[a].position - _transform[a + 1].position).magnitude;
        }
    }
    /// <summary>
    /// Called when the timer is completed.
    /// 
    /// Defines what happens when the timer has completed.
    /// </summary>
    public virtual void TimedAction() { }
    /// <summary>
    /// Defines what happens when a connected console is hacked
    /// </summary>
    public virtual void ButtonDown()
    {
        if (!_activated) {
            _activated = true;
            Init();
        }
    }

    public void ButtonUp()
    {

    }

    /// <summary>
    /// Draws lines between the checkpoints that the platform moves through.
    /// </summary>
    protected virtual void OnDrawGizmosSelected() {
        
        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent) {
            if (child != transform) {
                _transform.Add(child);
            }
        }
        for (int a  = 0; a < _transform.Count;a++) 
        {
            if (a + 1 != _transform.Count) {
                Gizmos.color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1);
                Gizmos.DrawLine(_transform[a].position,_transform[a+1].position);
                }
        }
    }
}
