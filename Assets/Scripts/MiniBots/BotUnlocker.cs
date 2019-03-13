using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnlocker : MonoBehaviour
{
    [SerializeField]
    private MiniBotType _bot = MiniBotType.HackBot;
    [SerializeField]
    private bool _restockBots = true;

    private void OnTriggerEnter(Collider other)
    {
        DeployBots dep = other.GetComponentInChildren<DeployBots>();

        dep.UnlockBot(_bot);

        if (_restockBots)
        {
            dep.Restock();
        }

        gameObject.SetActive(false);
    }
}