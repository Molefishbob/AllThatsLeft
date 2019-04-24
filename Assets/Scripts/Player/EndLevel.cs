using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : Collectible
{
    protected override void CollectAction()
    {
        GameManager.Instance.NextLevel();
    }
}