using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDispenser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.CanRestockBots = true;
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.CanRestockBots = false;
    }
}