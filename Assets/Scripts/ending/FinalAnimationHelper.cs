using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAnimationHelper : MonoBehaviour
{

    public Animator mainCharAnimator;
    public string runBoolName = "Run";
    public bool run = false;
    [SerializeField]
    private GameObject _textField = null;
    public Animator creditsAnimator;
    public string creditsBoolName = "credits";
    public bool showCredits = false;
    public GameObject _endScreen;
    private UnscaledOneShotTimer _timer = null;
    private UnscaledOneShotTimer _timer2 = null;
    private float _noInputTime = 0.25f;
    private float _hideTime = 5f;
    private bool _touchIsOkay = false;

    private void Start()
    {
        _textField.SetActive(false);
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _timer2 = gameObject.AddComponent<UnscaledOneShotTimer>();
        _timer2.OnTimerCompleted += InputMe;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (_textField.activeSelf && _touchIsOkay)
            {
                EndScreen();
            }
            else
            {
                _textField.SetActive(true);
                _timer2.StartTimer(_noInputTime);
            }
        }
    }
    private void InputMe()
    {
        _touchIsOkay = true;
        _timer2.StartTimer(_hideTime - _noInputTime);
        _timer2.OnTimerCompleted -= InputMe;
        _timer2.OnTimerCompleted += HideMe;
    }

    private void HideMe()
    {
        _textField.SetActive(false);
        _touchIsOkay = false;
        _timer2.OnTimerCompleted -= HideMe;
        _timer2.OnTimerCompleted += InputMe;
    }

    public void StartRun() {
        run = true;
        mainCharAnimator.SetBool(runBoolName, run);
    }

    public void StartCredits() {
        showCredits = true;
        _timer.StartTimer(creditsAnimator.runtimeAnimatorController.animationClips[0].length);
        _timer.OnTimerCompleted += EndScreen;
        creditsAnimator.SetBool(creditsBoolName, showCredits);
    }

    private void EndScreen()
    {
        GetComponent<Animator>().enabled = false;
        _endScreen.SetActive(true);
        _timer.OnTimerCompleted -= EndScreen;
    }

    private void OnDisable()
    {
        if (_timer != null) _timer.OnTimerCompleted -= EndScreen;
        if (_timer2 != null)
        {
            _timer2.OnTimerCompleted -= HideMe;
            _timer2.OnTimerCompleted -= InputMe;
        }
    }

}
