using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public string BotAmountFormat = "{0} / {1}";
    [SerializeField]
    private TMP_Text _botAmount;
    [SerializeField]
    private Image _miniBotImage;
    private MiniBotType _currentBot;
    [SerializeField]
    private Sprite _HackBot, _BombBot, _TrampBot;
    private int _maxBotAmount, _currentBotAmount;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.OnBotAmountChanged += SetBotAmount;
        GameManager.Instance.OnCurrentBotChanged += SetCurrentBot;
        GameManager.Instance.OnMaximumBotAmountChanged += SetMaxBotAmount;

        _maxBotAmount = GameManager.Instance.MaximumBotAmount;
        _currentBotAmount = GameManager.Instance.CurrentBotAmount;
        _currentBot = GameManager.Instance.CurrentBot;
    }

    private void Start() {
        SetBotAmount(_currentBotAmount);
        SetCurrentBot(_currentBot);
        SetMaxBotAmount(_maxBotAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetBotAmount(int amount) {
        _currentBotAmount = amount;
        _botAmount.text = string.Format(BotAmountFormat, amount,_maxBotAmount);
    }
    void SetCurrentBot(MiniBotType bot) {
        _currentBot = bot;

        switch (bot) {
            case MiniBotType.BombBot:
                _miniBotImage.sprite = _BombBot;
                break;
            case MiniBotType.HackBot:
                _miniBotImage.sprite = _HackBot;
                break;
            case MiniBotType.TrampBot:
                _miniBotImage.sprite = _TrampBot;
                break;
        }
    }

    void SetMaxBotAmount(int amount) {
        _maxBotAmount = amount;
        _botAmount.text = string.Format(BotAmountFormat, _currentBotAmount,amount);
    }

    private void OnDestroy() {
        GameManager.Instance.OnBotAmountChanged -= SetBotAmount;
        GameManager.Instance.OnCurrentBotChanged -= SetCurrentBot;
        GameManager.Instance.OnMaximumBotAmountChanged -= SetMaxBotAmount;
    }
}
