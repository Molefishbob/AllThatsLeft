using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPole : MonoBehaviour
{
    public int id;
    public Transform SpawnPoint;
    public ParticleSystem _particlePrefab;
    public Transform _particleSpawn;

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.LevelManager.SetCheckpoint(this))
        {
            if (GameManager.Instance.BeaconParticle == null)
            {
                GameManager.Instance.BeaconParticle = Instantiate(_particlePrefab, _particleSpawn.position, _particleSpawn.rotation);
                DontDestroyOnLoad(GameManager.Instance.BeaconParticle.gameObject);
            }
            else
            {
                GameManager.Instance.BeaconParticle.transform.position = _particleSpawn.position;
                GameManager.Instance.BeaconParticle.transform.rotation = _particleSpawn.rotation;
                GameManager.Instance.BeaconParticle.gameObject.SetActive(true);
            }
        }
    }
}
