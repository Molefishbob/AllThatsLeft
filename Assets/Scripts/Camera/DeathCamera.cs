using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCamera : MonoBehaviour
{
    [SerializeField]
    private Image _darkness = null;
    [SerializeField]
    private float _darkTime = 1.0f;

    private void Awake()
    {
        BrightnessComes();
        GameManager.Instance.Player.OnPlayerDeath += DarknessFalls;
        GameManager.Instance.Player.OnPlayerAlive += BrightnessComes;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            GameManager.Instance.Player.OnPlayerDeath -= DarknessFalls;
            GameManager.Instance.Player.OnPlayerAlive -= BrightnessComes;
        }
    }

    private void DarknessFalls()
    {
        gameObject.SetActive(true);
        Color fixColor = _darkness.color;
        fixColor.a = 1;
        _darkness.color = fixColor;
        _darkness.CrossFadeAlpha(0, 0, true);
        _darkness.CrossFadeAlpha(1, _darkTime, true);
    }

    private void BrightnessComes()
    {
        gameObject.SetActive(false);
    }
}