using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogSpawner : MonoBehaviour
{
    private EnemyDirection _eD;
    private bool _hasSpawned = false;

    private void Awake()
    {
        _eD = GetComponentInChildren<EnemyDirection>(true);
    }

    public void Spawn()
    {
        _eD._enemy = GameManager.Instance.FrogEnemyPool.GetObject();
        _eD._enemy._eDirect = _eD;
        _eD._enemy.transform.position = _eD.GetRandomTarget();
        _eD._enemy.transform.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
        _eD.SetRandomTarget();
        _eD._enemy.SetControllerActive(true);
        _eD._enemy._spawner = this;
        _eD._enemy.StopMoving = false;
        _hasSpawned = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSpawned)
        {
            Spawn();
        }
    }
}
