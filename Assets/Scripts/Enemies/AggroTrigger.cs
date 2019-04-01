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
        if (other.gameObject.layer == 10 || other.gameObject.layer == 9)
        {
            _frog.AggroEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 9)
        {
            _frog.AggroStay(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 9)
        {
            _frog.AggroExit();
        }
    }
}
