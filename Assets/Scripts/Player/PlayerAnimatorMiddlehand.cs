using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorMiddlehand : MonoBehaviour
{
    private DeployControlledBots _deployScript = null;
    private void Awake()
    {
        _deployScript = GetComponentInParent<DeployControlledBots>();
    }
    public void ThrowBot()
    {
        _deployScript.DeployBot();
    }
}