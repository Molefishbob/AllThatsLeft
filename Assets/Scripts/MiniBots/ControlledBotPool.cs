using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledBotPool : ObjectPool<BotMovement>
{
    protected override void OnDisable()
    {
        foreach (BotMovement bot in _pool)
        {
            bot.GetComponent<BotReleaser>().DisableAction();
        }
    }
}
