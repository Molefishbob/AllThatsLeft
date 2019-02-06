using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PauseableObject : MonoBehaviour, IPauseable
{
    protected bool _paused;

    protected virtual void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    protected virtual void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    public virtual void Pause()
    {
        _paused = true;
    }

    public virtual void UnPause()
    {
        _paused = false;
    }
}