using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledBotPool : ObjectPool<BotMovement>
{
    [SerializeField] private float forceReleaseTime = 5;
    private float forceReleaseTimer;
    [SerializeField] private bool anyBotActive;
    [SerializeField] private List<BotReleaser> bots;
    [SerializeField] private List<BotReleaser> activeBots;

    private void Start()
    {
        bots = new List<BotReleaser>();
        activeBots = new List<BotReleaser>();
    }

    private void LateUpdate()
    {
        if (bots.Count < transform.childCount)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                bots.Add(transform.GetChild(i).GetComponent<BotReleaser>());
            }
        }

        anyBotActive = false;
        activeBots.Clear();

        foreach (BotReleaser o in bots)
        {
            if (o.gameObject.activeInHierarchy && !o._selfTrampoline._bActing) // We don't count trampolines
            {
                anyBotActive = true;
                activeBots.Add(o);
            }
        }

        bool countTime = false;
        if (activeBots.Count == 0)
        {
            foreach (BotReleaser o in bots)
            {
                if (GameManager.Instance.Camera.LookingAt == o.ThisCameraTarget)
                {
                    countTime = true;
                }
            }
        }

        if (anyBotActive)
        {
            foreach (BotReleaser o in activeBots)
            {
                if (!o._bCanAct)
                {
                    countTime = true;
                }
            }
        }
        else if (!countTime)
        {
            forceReleaseTimer = 0;
        }

        if (countTime)
            forceReleaseTimer += Time.deltaTime;

        if (forceReleaseTimer >= forceReleaseTime)
        {
            bots[0].ReleaseInstant();
            Debug.LogWarning("Camera got stuck on bot somehow. Releasing");
        }
    }
}
