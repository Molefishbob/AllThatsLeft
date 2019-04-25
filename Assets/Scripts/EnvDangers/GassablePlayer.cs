using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GassablePlayer : GassableUnit
{
    [SerializeField]
    private Canvas _poisonScreen = null;

    protected override void EnterGasExtras()
    {
        _poisonScreen.gameObject.SetActive(true);
    }

    protected override void ExitGasExtras()
    {
        _poisonScreen.gameObject.SetActive(false);
    }
}
