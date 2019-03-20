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
    [SerializeField]
    private Image _miniBotImage = null;
    private MiniBotType _currentBot = MiniBotType.HackBot;
    [SerializeField]
    private Sprite _HackBot = null, _BombBot = null, _TrampBot = null;
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
        SetCurrentBot(_currentBot);
        SetMaxBotAmount(_maxBotAmount);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetBotAmount(int amount)
    {
        _currentBotAmount = amount;
        _botAmount.text = string.Format(BotAmountFormat, amount, _maxBotAmount);
    }
    void SetCurrentBot(MiniBotType bot)
    {
        _currentBot = bot;

        switch (bot)
        {
            case MiniBotType.HackBot:
                _miniBotImage.sprite = _HackBot;
                break;
            case MiniBotType.BombBot:
                _miniBotImage.sprite = _BombBot;
                break;
            case MiniBotType.TrampBot:
                _miniBotImage.sprite = _TrampBot;
                break;
        }
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
