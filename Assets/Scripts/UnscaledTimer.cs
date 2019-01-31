using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimer : Timer {
    protected override void Update() {
        if(IsRunning) {
            _timer += Time.unscaledDeltaTime;
            if(_timer >= Duration) {
                IsRunning = false;
                _timedObject.TimedAction();
            }
        }
    }
}