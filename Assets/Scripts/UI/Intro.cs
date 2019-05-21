using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    [SerializeField]
    private GameObject _textField = null;
    private VideoPlayer _introVideo = null;
    private float _videoLength = 0;
    private UnscaledOneShotTimer _timer = null;
    private UnscaledOneShotTimer _timer2 = null;
    private float _noInputTime = 0.25f;
    private float _hideTime = 5f;
    private bool _touchIsOkay = false;

    void Start()
    {
        _textField.SetActive(false);
        _timer = gameObject.AddComponent<UnscaledOneShotTimer>();
        _timer2 = gameObject.AddComponent<UnscaledOneShotTimer>();
        _introVideo = GetComponent<VideoPlayer>();
        _videoLength = (float)_introVideo.clip.length;
        _introVideo.Play();
        _timer.StartTimer(_videoLength);
        _timer.OnTimerCompleted += NextLevel;
        _timer2.OnTimerCompleted += InputMe;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (_textField.activeSelf && _touchIsOkay)
            {
                NextLevel();
            }
            else
            {
                _textField.SetActive(true);
                _timer2.StartTimer(_noInputTime);
            }
        }
    }

    private void NextLevel()
    {
        GameManager.Instance.NextLevel();
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

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.OnTimerCompleted -= NextLevel;
        if (_timer2 != null)
        {
            _timer2.OnTimerCompleted -= InputMe;
            _timer2.OnTimerCompleted -= HideMe;
        }
    }
}
