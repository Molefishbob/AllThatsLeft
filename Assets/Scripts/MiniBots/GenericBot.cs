using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBot : MonoBehaviour, IPauseable, ITimedAction
{
    public float _fSpeed = 5;
    public float _fLifetime = 5;
    [SerializeField]
    private bool _bMoving;
    private bool _bPaused;
    private CharacterController _charCon;
    private OneShotTimer _timer;

    void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _timer = GetComponent<OneShotTimer>();
    }

    void Start()
    {
        AddToPauseCollection();
        StartMovement();
    }

    void Update()
    {
        if (!_bPaused)
        {
            if (_bMoving)
            {
                Vector3 movement = transform.forward * _fSpeed * Time.deltaTime;
                movement += Physics.gravity * Time.deltaTime;
                _charCon.Move(movement);
            }
        }
    }

    void StartMovement()
    {
        _bMoving = true;
        _timer.StartTimer(_fLifetime, this);
        // Animations and stuff here
    }

    void ResetBot()
    {

    }

    public void Pause()
    {
        _bPaused = true;
    }

    public void UnPause()
    {
        _bPaused = false;
    }

    public void AddToPauseCollection()
    {
        GameManager.Instance.Pauseables.Add(this);
    }

    public void RemoveFromPauseCollection()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.Pauseables.Remove(this);
    }

    protected virtual void OnDestroy()
    {
        RemoveFromPauseCollection();
    }

    public void TimedAction()
    {
        gameObject.SetActive(false);
        //Play animations explode
    }
}
