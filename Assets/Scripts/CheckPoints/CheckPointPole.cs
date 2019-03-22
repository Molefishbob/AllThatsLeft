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
    private Collider _collider;

    void Awake()
    {
        _SpawnPoint = transform.GetChild(0);
        _collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.LevelManager.SetCheckpoint(id);
        _collider.enabled = false;
    }
}
