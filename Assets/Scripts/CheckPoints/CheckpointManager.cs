using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    public CheckPointPole _currentCheckPoint{get;private set;}

    public Vector3 GetSpawnLocation()
    {
        if (_currentCheckPoint = null)
            return Vector3.zero;
        else
            return _currentCheckPoint.SpawnPoint.position;
    }

    public void SetCheckPoint(CheckPointPole cpp)
    {
        _currentCheckPoint = cpp;
    }
}
