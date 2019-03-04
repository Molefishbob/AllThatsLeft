using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployBots : MonoBehaviour, IPauseable, ITimedAction
{
    public string _scrollButton = "Scroll";
    public string _selectBot1Button = "Bot1";
    public string _selectBot2Button = "Bot2";
    public string _selectBot3Button = "Bot3";
    //public string _selectBot4Button = "Bot4";
    //public string _selectBot5Button = "Bot5";
    public string _deployBotButton = "Deploy Bot";
    public float _buttonDelay = 0.1f;
    public float _deployDelay = 2.0f;
    public Transform _deployTarget;
    public float _extraSpaceRequired = 0.1f;
    public LayerMask _deployCollisionLayers;
    public LayerMask _deployableTerrain;
    public float _deployHeightRange = 1.0f;
    public int _startingBotAmount;

    // temporary
    public Material _blueMaterial;
    public Material _redMaterial;

    private bool _paused = false;
    private int _selectedBot = 0;
    private int _numTypes = 3;
    private ThirdPersonPlayerMovement _player;
    private OneShotTimer _timer;
    private Renderer _indicator;
    private Vector3 _deployStartPosition;
    private GenericBot _heldBot;
    private HackPool _hackBotPool;
    private BombPool _bombBotPool;
    private TrampPool _jumpBotPool;

    public int BotAmount
    {
        get;
        private set;
    }

    private void Awake()
    {
        BotAmount = _startingBotAmount;
        _player = GetComponentInParent<ThirdPersonPlayerMovement>();
        _timer = GetComponent<OneShotTimer>();
        _indicator = _deployTarget.GetComponentInChildren<Renderer>();
        _hackBotPool = FindObjectOfType<HackPool>();
        _bombBotPool = FindObjectOfType<BombPool>();
        _jumpBotPool = FindObjectOfType<TrampPool>();
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
        _timer.SetTimerTarget(this);
        _deployStartPosition = _deployTarget.localPosition;
    }

    private void Update()
    {
        if (!_paused)
        {
            if (_heldBot != null)
            {
                if (_player.IsGrounded)
                {
                    RaycastHit hit;
                    CharacterController heldController = _heldBot.GetComponent<CharacterController>();
                    Vector3 upVector = -Physics.gravity.normalized;
                    if (Physics.Raycast(
                            _deployTarget.parent.TransformPoint(_deployStartPosition) + upVector * _deployHeightRange,
                            Physics.gravity,
                            out hit,
                            2 * _deployHeightRange,
                            _deployableTerrain))
                    {
                        _deployTarget.position = hit.point;

                        if (!Physics.CheckCapsule(
                            _deployTarget.position + upVector * (heldController.radius + heldController.skinWidth + _extraSpaceRequired),
                            _deployTarget.position + upVector * (heldController.height - heldController.radius - _extraSpaceRequired),
                            heldController.radius + _extraSpaceRequired,
                            _deployCollisionLayers))
                        {
                            // TODO: make indicator blue here
                            _indicator.enabled = true;
                            _indicator.material = _blueMaterial;

                            if (!_timer.IsRunning && Input.GetButtonDown(_deployBotButton))
                            {
                                _heldBot.transform.parent = null;
                                _heldBot.transform.position = _deployTarget.position;
                                _heldBot.transform.rotation = _deployTarget.rotation;
                                _heldBot.StartMovement();
                                _heldBot.SetControllerActive(true);
                                _heldBot = null;
                                BotAmount--;
                                _timer.StartTimer(_deployDelay);
                            }
                        }
                        else
                        {
                            // TODO: make indicator red here
                            _indicator.enabled = true;
                            _indicator.material = _redMaterial;
                        }
                    }
                    else
                    {
                        _deployTarget.localPosition = _deployStartPosition;
                        // TODO: make indicator red here
                        _indicator.enabled = true;
                        _indicator.material = _redMaterial;
                    }
                }
                else
                {
                    _deployTarget.localPosition = _deployStartPosition;
                    // TODO: make indicator red here
                    _indicator.enabled = true;
                    _indicator.material = _redMaterial;
                }
            }
            else
            {
                // TODO: hide indicator here
                _indicator.enabled = false;
            }

            if (BotAmount > 0 && !_timer.IsRunning)
            {
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
                /* else if (Input.GetButtonDown(_selectBot4Button))
                {
                    selection = 3;
                }
                else if (Input.GetButtonDown(_selectBot5Button))
                {
                    selection = 4;
                } */
                else if (Input.GetAxisRaw(_scrollButton) > 0.0f)
                {
                    selection++;
                    if (selection >= _numTypes)
                    {
                        selection = 0;
                    }
                }
                else if (Input.GetAxisRaw(_scrollButton) < 0.0f)
                {
                    selection--;
                    if (selection < 0)
                    {
                        selection = _numTypes - 1;
                    }
                }

                if (selection != -1)
                {
                    _selectedBot = selection;

                    if (_heldBot != null)
                    {
                        _heldBot.ResetBot();
                        _heldBot = null;
                    }

                    switch (_selectedBot)
                    {
                        case 0:
                            _heldBot = _hackBotPool.GetObject();
                            break;
                        case 1:
                            _heldBot = _bombBotPool.GetObject();
                            break;
                        case 2:
                            _heldBot = _jumpBotPool.GetObject();
                            break;
                        default:
                            Debug.LogError("INVALID BOT ID");
                            break;
                    }

                    _heldBot.SetControllerActive(false);
                    _heldBot.transform.parent = transform;
                    _heldBot.transform.position = transform.position;
                    _heldBot.transform.rotation = transform.rotation;

                    _timer.StartTimer(_buttonDelay);
                }
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

    }
}