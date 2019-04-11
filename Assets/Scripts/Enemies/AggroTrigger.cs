using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
    private FrogEnemy _frog;

    private void Awake()
    {
        _frog = GetComponentInParent<FrogEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _frog.AggroEnter();
    }

    private void OnTriggerStay(Collider other)
    {
        _frog.AggroStay(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        _frog.AggroExit();
    }
}
