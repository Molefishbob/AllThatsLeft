using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenLoop : GenericMover
{
    protected override Vector3 InternalMove()
    {
        if (_activated)
        {
            float currLength = (_timer.TimeElapsed) / (_timer.Duration) * _length;

            for (int i = 0; i < _currentObjectNum; ++i)
            {
                currLength -= (_transform[i].position - _transform[i + 1].position).magnitude;
            }

            _fracTime = currLength / (_transform[_currentObjectNum].position - _transform[_nextObjectNum].position).magnitude;

            Vector3 pos = Vector3.Lerp(_transform[_currentObjectNum].position, _transform[_nextObjectNum].position, _fracTime);

            if (_fracTime >= 1)
            {
                ChangeTarget();
            }

            return pos;
        }

        return transform.position;
    }

    /// <summary>
    /// Changes the direction the object is travelling
    /// </summary>
    private void ChangeTarget()
    {
        _currentObjectNum = _nextObjectNum;
        _nextObjectNum = _currentObjectNum + 1;
        if (_nextObjectNum >= _transform.Count)
        {
            _nextObjectNum = 0;
        }
    }

    /// <summary>
    /// Initializes the script.
    ///
    /// Adds the distance between the last and the first object to the complete length.
    /// </summary>
    public override void Init()
    {
        base.Init();
        _length += (_transform[0].position - _transform[_amountOfTransforms].position).magnitude;
    }

    /// <summary>
    /// Called when the timer is completed.
    ///
    /// Defines what happens when the timer has completed.
    /// </summary>
    protected override void TimedAction()
    {
        _nextObjectNum = 1;
        _currentObjectNum = 0;
    }

    /// <summary>
    /// Draws lines between the checkpoints that the platform moves through.
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        if (transform.parent == null) return;

        _transform = new List<Transform>(transform.parent.childCount);

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                _transform.Add(child);
            }
        }
        for (int a = 0; a < _transform.Count; a++)
        {
            if (a + 1 != _transform.Count)
            {
                Gizmos.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                Gizmos.DrawLine(_transform[a].position, _transform[a + 1].position);
            }
            else
            {
                Gizmos.DrawLine(_transform[a].position, _transform[0].position);
            }
        }
    }
}
