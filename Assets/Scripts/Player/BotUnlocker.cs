using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUnlocker : Collectible
{
    protected override void Start()
    {
        base.Start();

        if (PrefsManager.Instance.BotsUnlocked)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void CollectAction()
    {
        PrefsManager.Instance.BotsUnlocked = true;
    }
}