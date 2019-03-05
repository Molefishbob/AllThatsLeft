using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMover : MonoBehaviour, ITimedAction, IButtonInteraction
{

    [Tooltip("The amount of time it takes to go the whole length")]
    public float _duration;
    [SerializeField,Tooltip("The amount of time the platform is still at the ends of the route")]
    protected float _stopTime;
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

    private void Awake()
    {
        _timer = GetComponent<RepeatingTimer>();
        _transform.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        _timer.SetTimerTarget(this);
        _timer.StartTimer(_duration + _stopTime);

        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent) {
            if (child != transform) {
                _transform.Add(child);
            }
        }
        
        _amountOfTransforms = _transform.Count - 1;

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
        _activated = true;
        Init();
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
