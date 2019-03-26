using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer : MonoBehaviour
{
    public event GenericEvent OnTimerCompleted;
    protected float _timer = 0.0f;
    private float _duration = 0.0f;

    /// <summary>
    /// The time the timer takes to complete.
    /// </summary>
    /// <value>
    /// Only get is public, use StartTimer to set the duration.
    /// </value>
    public float Duration
    {
        get
        {
            return _duration;
        }
        protected set
        {
            if (value < 0.0f)
            {
                Debug.LogError("Invalid duration!!!");
            }
            else
            {
                _duration = value;
            }
        }
    }

    /// <summary>
    /// Time elapsed in seconds since the timer started.
    /// </summary>
    /// <value>
    /// Get only.
    /// </value>
    public float TimeElapsed { get { return _timer; } }

    /// <summary>
    /// Time left in the timer in seconds.
    /// </summary>
    /// <value>
    /// Get only.
    /// </value>
    public float TimeLeft { get { return Duration - _timer; } }

    /// <summary>
    /// Time elapsed since the timer started normalized to 0-1.
    /// </summary>
    /// <value>
    /// Get only.
    /// </value>
    public float NormalizedTimeElapsed { get { return TimeElapsed / Duration; } }

    /// <summary>
    /// Time left in the timer normalized to 0-1.
    /// </summary>
    /// <value>
    /// Get only.
    /// </value>
    public float NormalizedTimeLeft { get { return TimeLeft / Duration; } }

    /// <summary>
    /// True when the timer is running.
    /// </summary>
    public bool IsRunning { get; protected set; }

    protected void CompletedTimer()
    {
        if (OnTimerCompleted != null)
        {
            OnTimerCompleted();
        }
    }

    /// <summary>
    /// Starts the timer from zero.
    /// </summary>
    /// <param name="duration">Timer duration in seconds.</param>
    public void StartTimer(float duration)
    {
        Duration = duration;
        if (Duration <= 0.0f)
        {
            Debug.LogWarning("Timer duration is " + Duration + ", are you sure this is what you wanted?");
        }
        _timer = 0.0f;
        IsRunning = true;
    }

    /// <summary>
    /// Resume a stopped timer.
    /// </summary>
    public void ResumeTimer()
    {
        if (_timer < Duration)
        {
            IsRunning = true;
        }
        else
        {
            Debug.LogWarning("Timer has already completed. Start timer again instead.");
        }
    }

    /// <summary>
    /// Stop the timer.
    /// </summary>
    public void StopTimer()
    {
        IsRunning = false;
    }
}