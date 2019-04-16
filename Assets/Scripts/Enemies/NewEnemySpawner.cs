using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    private EnemyDirection _eD;

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
        Spawn();
    }
}
