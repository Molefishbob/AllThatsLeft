using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnlocker : MonoBehaviour
{
    [SerializeField]
    private MiniBotType _bot = MiniBotType.HackBot;

    private void OnTriggerEnter(Collider other)
    {
        GetComponentInChildren<DeployBots>().UnlockBot(_bot);
        gameObject.SetActive(false);
    }
}