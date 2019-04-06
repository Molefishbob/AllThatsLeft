using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject _ball;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        if (GameManager.Instance.Player != null)
            GameManager.Instance.Player.ControlsDisabled = true;

        PrefsManager.Instance.AudioMuteSFX = true;
    }
}