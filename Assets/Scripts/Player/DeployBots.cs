using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployBots : MonoBehaviour, IPauseable, ITimedAction
{
    [SerializeField]
    private string _scrollButton = "Scroll";
    [SerializeField]
    private string _selectBot1Button = "Bot1";
    [SerializeField]
    private string _selectBot2Button = "Bot2";
    [SerializeField]
    private string _selectBot3Button = "Bot3";
    [SerializeField]
    private string _deployBotButton = "Deploy Bot";
    [SerializeField]
    private string _takeOutBotButton = "Take Out Bot";
    [SerializeField]
    private string _useObjectButton = "Use Object";
    [SerializeField]
    private float _deployDelay = 2.0f;
    [SerializeField]
    private Transform _deployTarget = null;
    [SerializeField]
    private float _extraSpaceRequired = 0.1f;
    [SerializeField]
    private LayerMask _deployCollisionLayers = 0;
    [SerializeField]
    private LayerMask _deployableTerrain = 0;
    [SerializeField]
    private float _deployHeightRange = 1.0f;
    [SerializeField]
    private int _startingBotAmount = 10;
    [SerializeField]
    private int _startingMaxBotAmount = 10;
    [SerializeField]
    private float _automaticPutAwayDelay = 10.0f;
    [SerializeField]
    private string _botAssCheeks = "LowerBody";
    [SerializeField]
    private MiniBotType[] _botOrder = { MiniBotType.HackBot, MiniBotType.BombBot, MiniBotType.TrampBot };

    // temporary
    public Material _blueMaterial;
    public Material _redMaterial;

    private bool _shouldDeployBot = false;
    private bool _paused = false;
    private int _selectedBot = 0;
    private OneShotTimer _deployDelayTimer;
    private OneShotTimer _autoAwayTimer;
    private bool _scrollUsable = true;
    private Renderer[] _indicators;
    private Vector3 _deployStartPosition;
    private GenericBot _heldBot;
    private Transform _heldBotAssCheeks;
    private HackPool _hackBotPool;
    private BombPool _bombBotPool;
    private TrampPool _jumpBotPool;
    private HashSet<MiniBotType> _unlockedBotTypes = new HashSet<MiniBotType>();

    private void Awake()
    {
        OneShotTimer[] timers = GetComponents<OneShotTimer>();
        _deployDelayTimer = timers[0];
        _autoAwayTimer = timers[1];
        _indicators = _deployTarget.GetComponentsInChildren<Renderer>();
        _hackBotPool = FindObjectOfType<HackPool>();
        _bombBotPool = FindObjectOfType<BombPool>();
        _jumpBotPool = FindObjectOfType<TrampPool>();

        //TODO: remove unlocks
        if (_hackBotPool != null) UnlockBot(MiniBotType.HackBot);
        if (_bombBotPool != null) UnlockBot(MiniBotType.BombBot);
        if (_jumpBotPool != null) UnlockBot(MiniBotType.TrampBot);
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);

        _autoAwayTimer.SetTimerTarget(this);
        _deployStartPosition = _deployTarget.localPosition;

        GameManager.Instance.CurrentBotAmount = _startingBotAmount;
        GameManager.Instance.MaximumBotAmount = _startingMaxBotAmount;
        GameManager.Instance.CurrentBot = MiniBotType.HackBot;

        HideIndicator();
    }

    private void Update()
    {
        if (!_paused)
        {
            if(Input.GetButtonDown(_useObjectButton) && GameManager.Instance.CanRestockBots)
            {
                Restock();
            }
            else if (_unlockedBotTypes.Count > 0)
            {
                bool buttonPressed = false;
                int selection = -1;

                if (Input.GetButtonDown(_selectBot1Button))
                {
                    selection = 0;
                }
                else if (Input.GetButtonDown(_selectBot2Button))
                {
                    selection = 1;
                }
                else if (Input.GetButtonDown(_selectBot3Button))
                {
                    selection = 2;
                }

                if (selection != -1 && !_unlockedBotTypes.Contains(_botOrder[selection]))
                {
                    selection = -1;
                }

                float currentScroll = Input.GetAxis(_scrollButton);

                if (_scrollUsable)
                {
                    if (currentScroll > 0.5f)
                    {
                        selection = _selectedBot + 1;
                        if (selection >= _botOrder.Length)
                        {
                            selection = 0;
                        }
                        while (!_unlockedBotTypes.Contains(_botOrder[selection]))
                        {
                            selection++;
                            if (selection >= _botOrder.Length)
                            {
                                selection = 0;
                            }
                        }

                        _scrollUsable = false;
                    }
                    else if (currentScroll < -0.5f)
                    {
                        selection = _selectedBot - 1;
                        if (selection < 0)
                        {
                            selection = _botOrder.Length - 1;
                        }
                        while (!_unlockedBotTypes.Contains(_botOrder[selection]))
                        {
                            selection--;
                            if (selection < 0)
                            {
                                selection = _botOrder.Length - 1;
                            }
                        }

                        _scrollUsable = false;
                    }
                }
                else if (Mathf.Abs(currentScroll) <= 0.2f)
                {
                    _scrollUsable = true;
                }

                if (selection != -1)
                {
                    buttonPressed = true;
                    _selectedBot = selection;
                    GameManager.Instance.CurrentBot = _botOrder[_selectedBot];

                    PutAwayBot();
                }
                else if (_heldBot == null && GameManager.Instance.CurrentBotAmount > 0 && !_deployDelayTimer.IsRunning && Input.GetButtonDown(_takeOutBotButton))
                {
                    buttonPressed = true;
                    switch (GameManager.Instance.CurrentBot)
                    {
                        case MiniBotType.HackBot:
                            _heldBot = _hackBotPool.GetObject();
                            break;
                        case MiniBotType.BombBot:
                            _heldBot = _bombBotPool.GetObject();
                            break;
                        case MiniBotType.TrampBot:
                            _heldBot = _jumpBotPool.GetObject();
                            break;
                        default:
                            Debug.LogError("INVALID BOT ID");
                            break;
                    }

                    Animator animi = _heldBot.GetComponentInChildren<Animator>();
                    if (animi != null)
                    {
                        _heldBotAssCheeks = animi.transform.GetChild(0).Find(_botAssCheeks);
                    }
                    if (_heldBotAssCheeks == null)
                    {
                        _heldBotAssCheeks = _heldBot.transform;
                    }

                    _autoAwayTimer.StartTimer(_automaticPutAwayDelay);
                }

                if (_heldBot == null)
                {
                    HideIndicator();
                }
                else if (GameManager.Instance.Player.IsGrounded)
                {
                    RaycastHit hit;
                    Vector3 upVector = -Physics.gravity.normalized;
                    if (Physics.Raycast(
                            _deployTarget.parent.TransformPoint(_deployStartPosition) + upVector * _deployHeightRange,
                            Physics.gravity,
                            out hit,
                            2 * _deployHeightRange,
                            _deployableTerrain))
                    {
                        _deployTarget.position = hit.point;

                        if (Physics.CheckCapsule(
                            _deployTarget.position + upVector * (_heldBot.Radius + _heldBot.SkinWidth + _extraSpaceRequired),
                            _deployTarget.position + upVector * (_heldBot.Height - _heldBot.Radius - _extraSpaceRequired),
                            _heldBot.Radius + _extraSpaceRequired,
                            _deployCollisionLayers))
                        {
                            ShowInvalidIndicator();
                        }
                        else if (!buttonPressed && !_deployDelayTimer.IsRunning && Input.GetButtonDown(_deployBotButton))
                        {
                            _shouldDeployBot = true;
                            HideIndicator();
                        }
                        else
                        {
                            ShowValidIndicator();
                        }
                    }
                    else
                    {
                        _deployTarget.localPosition = _deployStartPosition;
                        ShowInvalidIndicator();
                    }
                }
                else
                {
                    _deployTarget.localPosition = _deployStartPosition;
                    ShowInvalidIndicator();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (_heldBot != null)
        {
            _heldBot.transform.position = transform.position - _heldBotAssCheeks.position + _heldBot.transform.position;
            _heldBot.transform.rotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (!_paused)
        {
            if (_shouldDeployBot)
            {
                DeployBot();
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    public void TimedAction()
    {
        PutAwayBot();
    }

    private void PutAwayBot()
    {
        if (_heldBot != null)
        {
            _heldBot.ResetBot();
            _heldBot = null;
            _heldBotAssCheeks = null;
        }
    }

    private void DeployBot()
    {
        _shouldDeployBot = false;
        _heldBot.transform.position = _deployTarget.position;
        _heldBot.transform.rotation = _deployTarget.rotation;
        _heldBot.StartMovement();
        _heldBot = null;
        _heldBotAssCheeks = null;
        GameManager.Instance.CurrentBotAmount--;
        if(_deployDelay > 0.0f) _deployDelayTimer.StartTimer(_deployDelay, false);
    }

    private void ShowValidIndicator()
    {
        foreach (Renderer renderer in _indicators)
        {
            renderer.enabled = true;
            renderer.material = _blueMaterial;
        }
    }

    private void ShowInvalidIndicator()
    {
        foreach (Renderer renderer in _indicators)
        {
            renderer.enabled = true;
            renderer.material = _redMaterial;
        }
    }

    private void HideIndicator()
    {
        foreach (Renderer renderer in _indicators)
        {
            renderer.enabled = false;
        }
    }

    /// <summary>
    /// Restocks minibots by an amount.
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void Restock(int amount)
    {
        GameManager.Instance.CurrentBotAmount = Mathf.Clamp(GameManager.Instance.CurrentBotAmount + amount, 0, GameManager.Instance.MaximumBotAmount);
    }

    /// <summary>
    /// Restocks minibots to full.
    /// </summary>
    public void Restock()
    {
        GameManager.Instance.CurrentBotAmount = GameManager.Instance.MaximumBotAmount;
    }

    /// <summary>
    /// Unlocks the given bot type to be usable.
    /// </summary>
    /// <param name="type">MiniBotType</param>
    public void UnlockBot(MiniBotType type)
    {
        _unlockedBotTypes.Add(type);
        if (_unlockedBotTypes.Count == 1)
        {
            GameManager.Instance.CurrentBot = type;
        }
    }
}