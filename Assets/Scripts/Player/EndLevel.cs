using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : Collectible
{
    private int _shaderProperty;

    protected override void Awake()
    {
        base.Awake();
        _shaderProperty = Shader.PropertyToID("_cutoff");
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) StopEffects();
    }

    private void Update()
    {
        if (_timer.IsRunning)
        {
            GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, _timer.NormalizedTimeLeft);
        }
    }

    protected override void HoldAction()
    {
        GameManager.Instance.Player._teleportEffectSlow.Play();
    }

    protected override void CollectAction()
    {
        StopEffects();
        GameManager.Instance.NextLevel();
    }

    private void StopEffects()
    {
        GameManager.Instance.Player._renderer.material.SetFloat(_shaderProperty, 0.0f);
        GameManager.Instance.Player._teleportEffectSlow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
