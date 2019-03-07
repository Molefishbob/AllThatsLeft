using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Current checkpoint
    /// </summary>
    /// <value></value>
    public CheckPointPole _currentCheckPoint { get; set; }
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
}
