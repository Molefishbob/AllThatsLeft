using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPole : MonoBehaviour
{
    public int id;
    [HideInInspector]
    public Transform SpawnPoint { get; private set; }
    private Collider _collider;

    void Awake()
    {
        SpawnPoint = transform.GetChild(0);
        _collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.LevelManager.SetCheckpoint(id);
        _collider.enabled = false;
    }
}
