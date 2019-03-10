using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Current checkpoint
    /// </summary>
    /// <value></value>
    public CheckPointPole _currentCheckPoint { get; private set; }
    private CheckPointPole[] _allLevelCheckPoints;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public BombPool _bombPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public TrampPool _trampPoolPrefab;
    /// <summary>
    /// The pool prefab
    /// </summary>
    public HackPool _hackPoolPrefab;
    public bool _levelNeedsBombBots;
    public bool _levelNeedsTrampBots;
    public bool _levelNeedsHackBots;

    void Awake()
    {
        GameManager.Instance.LevelManager = this;
        if (_levelNeedsBombBots && GameManager.Instance.BombPool == null)
        {
            GameManager.Instance.BombPool = Instantiate(_bombPoolPrefab);
        }
        if (_levelNeedsTrampBots && GameManager.Instance.TrampPool == null)
        {
            GameManager.Instance.TrampPool = Instantiate(_trampPoolPrefab);
        }
        if (_levelNeedsHackBots && GameManager.Instance.HackPool == null)
        {
            GameManager.Instance.HackPool = Instantiate(_hackPoolPrefab);
        }
        _allLevelCheckPoints = FindObjectsOfType<CheckPointPole>();
        _allLevelCheckPoints = SortCheckpoints(_allLevelCheckPoints);
    }

    /// <summary>
    /// Gives the spawn location from current checkpoint.
    /// </summary>
    /// <returns>Spawn location</returns>
    public Vector3 GetSpawnLocation()
    {
        if (_currentCheckPoint = null)
            return Vector3.zero;
        else
            return _currentCheckPoint.SpawnPoint.position;
    }

    public void SetCheckpoint(CheckPointPole cp)
    {
        if (_currentCheckPoint == null)
        {
            _currentCheckPoint = cp;
        }
        else if (_currentCheckPoint.id < cp.id)
        {
            _currentCheckPoint = cp;
        }
        Debug.Log(_currentCheckPoint.id);
    }

    private CheckPointPole[] SortCheckpoints(CheckPointPole[] _cp)
    {
        bool sorted = false;
        while (!sorted)
        {
            bool swapped = false;
            for (int i = 0; i < _cp.Length - 1; i++)
            {
                if (_cp[i].id > _cp[i+1].id)
                {
                    CheckPointPole tmp = _cp[i];
                    _cp[i] = _cp[i+1];
                    _cp[i+1] = tmp;
                    swapped = true;
                } 
                else if (_cp[i].id == _cp[i+1].id)
                {
                    Debug.LogError("THERE ARE 2 OR MORE CHECKPOINTS WITH SAME ID!! FIX!!");
                    swapped = false;
                    break;
                }
            }
            sorted = !swapped;
        }
        return null;
    }
}
