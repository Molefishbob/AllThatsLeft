using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPole : MonoBehaviour
{
    [SerializeField]
    private LayerMask _PlayerLayer = 1 << 10;
    public int id;
    private Transform _SpawnPoint;
    /// <summary>
    /// The spawnpoint of the pole. Needs to be set in editor
    /// </summary>
    /// <value></value>
    public Transform SpawnPoint { get { return _SpawnPoint; } }

    void Awake()
    {
        _SpawnPoint = transform.GetChild(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == _PlayerLayer)
            GameManager.Instance.LevelManager.SetCheckpoint(this);
    }
}
