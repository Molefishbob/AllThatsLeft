using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BotActionBase : MonoBehaviour
{
    protected BotMovement _selfMover;
    public bool _bCanAct = false;
    protected bool _bPaused;

    protected virtual void Awake()
    {
        _selfMover = GetComponent<BotMovement>();
    }

    public abstract void DisableAction();
}
