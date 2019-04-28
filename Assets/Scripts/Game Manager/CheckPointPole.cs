using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPole : MonoBehaviour
{
    public int id;
    public Transform SpawnPoint;

    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private string _triggerName = "getChecked";

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.LevelManager.SetCheckpoint(this))
        {
            _animator.SetTrigger(_triggerName);
        }
    }
}
