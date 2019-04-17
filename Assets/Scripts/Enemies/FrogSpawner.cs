using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    private EnemyDirection _eD;
    private bool _hasSpawned = false;

    private void Awake()
    {
        _eD = GetComponentInChildren < EnemyDirection >();
     
    }

    public void Spawn()
    {
        _eD._enemy = GameManager.Instance.FrogEnemyPool.GetObject();
        _eD._enemy.transform.position = transform.position;
        _eD._enemy.transform.rotation = transform.rotation;
        _eD._enemy.SetControllerActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSpawned)
        {
            Spawn();
            _hasSpawned = true;
        }
    }
}
