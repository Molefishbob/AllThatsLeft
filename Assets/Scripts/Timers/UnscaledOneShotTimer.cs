using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledOneShotTimer : Timer {
    protected override void Update() {
        if(IsRunning) {
            _timer += Time.unscaledDeltaTime;
            if(_timer >= Duration) {
                _timer = Duration;
                CompletedTimer();
                _timedObject.TimedAction();
            }
        }
    }
    protected override void CompletedTimer() {
        IsRunning = false;
    }
}