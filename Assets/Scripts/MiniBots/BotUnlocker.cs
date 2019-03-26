using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnlocker : MonoBehaviour
{
    private void OnEnable()
    {
        if (PrefsManager.Instance.BotsUnlocked)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PrefsManager.Instance.BotsUnlocked = true;
        gameObject.SetActive(false);
    }
}