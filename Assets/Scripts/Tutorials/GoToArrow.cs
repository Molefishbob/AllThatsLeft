using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToArrow : MonoBehaviour
{
    [SerializeField]
    private GameObject _arrow = null;

    private Collider _collider = null;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _arrow.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement p = other.GetComponent<PlayerMovement>();
        if (p == null || p.ControlsDisabled) return;

        _collider.enabled = false;
        _arrow.SetActive(true);
    }
}