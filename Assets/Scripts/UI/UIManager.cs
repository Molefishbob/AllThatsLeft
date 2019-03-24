using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public string BotAmountFormat = "{0} / {1}";
    [SerializeField]
    private TMP_Text _botAmount = null;
    private int _maxBotAmount = 0, _currentBotAmount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.OnBotAmountChanged += SetBotAmount;
        GameManager.Instance.OnMaximumBotAmountChanged += SetMaxBotAmount;

        _maxBotAmount = GameManager.Instance.MaximumBotAmount;
        _currentBotAmount = GameManager.Instance.CurrentBotAmount;
    }

    private void Start()
    {
        SetBotAmount(_currentBotAmount);
        SetMaxBotAmount(_maxBotAmount);
    }

    void SetBotAmount(int amount)
    {
        _currentBotAmount = amount;
        _botAmount.text = string.Format(BotAmountFormat, amount, _maxBotAmount);
    }

    void SetMaxBotAmount(int amount)
    {
        _maxBotAmount = amount;
        _botAmount.text = string.Format(BotAmountFormat, _currentBotAmount, amount);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBotAmountChanged -= SetBotAmount;
            GameManager.Instance.OnMaximumBotAmountChanged -= SetMaxBotAmount;
        }
    }
}
