using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorMiddlehand : MonoBehaviour
{
    [SerializeField]
    private Transform _characterHand = null;
    [SerializeField]
    private SingleSFXSound _victorySound = null;

    private DeployControlledBots _deployScript = null;

    [HideInInspector]
    public Collectible _collectible = null;

    private void Awake()
    {
        _deployScript = GetComponentInParent<DeployControlledBots>();
    }

    public void ThrowBot()
    {
        _deployScript.DeployBot(_characterHand);
    }

    public void VictorySound()
    {
        _victorySound.PlaySound(false);
    }

    public void PoseComplete()
    {
        _collectible.HoldPose(_characterHand);
        _collectible = null;
    }
}
