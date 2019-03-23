using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : PlayerMovement, IDamageReceiver
{
    private PlayerBotInteractions _pbi;
    protected override void Awake()
    {
        base.Awake();
        _pbi = GetComponent<PlayerBotInteractions>();
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        // TODO animations
        _pbi.StopActing();
        if (_pbi._bActive) _pbi.ReleaseControls(true);
    }

    protected override void OutOfBounds()
    {
        _pbi.ReleaseControls(false);
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }
}
