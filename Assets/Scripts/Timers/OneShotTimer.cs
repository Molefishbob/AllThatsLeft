using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotTimer : ScaledTimer
{
    protected override void CompletedTimer()
    {
        IsRunning = false;
    }
}