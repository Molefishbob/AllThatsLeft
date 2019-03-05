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
    public string _takeOutBotButton = "Take Out Bot";
    public float _deployDelay = 2.0f;
    public Transform _deployTarget;
    public float _extraSpaceRequired = 0.1f;
    public LayerMask _deployCollisionLayers;
    public LayerMask _deployableTerrain;
    public float _deployHeightRange = 1.0f;
    public int _botAmount = 5;
    public int _numTypes = 3;
    public float _automaticPutAwayDelay = 10.0f;

    // temporary
    public Material _blueMaterial;
    public Material _redMaterial;

    private bool _paused = false;
    private int _selectedBot = 0;
    private OneShotTimer _deployDelayTimer;
    private OneShotTimer _autoAwayTimer;
    private bool _scrollUsable = true;
    private Renderer _indicator;
    private Vector3 _deployStartPosition;
    private GenericBot _heldBot;
    private HackPool _hackBotPool;
    private BombPool _bombBotPool;
    private TrampPool _jumpBotPool;

    private void Awake()
    {
        OneShotTimer[] timers = GetComponents<OneShotTimer>();
        _deployDelayTimer = timers[0];
        _autoAwayTimer = timers[1];
        _indicator = _deployTarget.GetComponentInChildren<Renderer>();
        _hackBotPool = FindObjectOfType<HackPool>();
        _bombBotPool = FindObjectOfType<BombPool>();
        _jumpBotPool = FindObjectOfType<TrampPool>();
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
        _autoAwayTimer.SetTimerTarget(this);
        _deployStartPosition = _deployTarget.localPosition;
    }

    private void FixedUpdate()
    {
        if (!_paused)
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
            /* else if (Input.GetButtonDown(_selectBot4Button))
            {
                selection = 3;
            }
            else if (Input.GetButtonDown(_selectBot5Button))
            {
                selection = 4;
            } */

            float currentScroll = Input.GetAxis(_scrollButton);

            if (_scrollUsable)
            {
                if (currentScroll > 0.5f)
                {
                    selection = _selectedBot + 1;
                    if (selection >= _numTypes)
                    {
                        selection = 0;
                    }
                    _scrollUsable = false;
                }
                else if (currentScroll < -0.5f)
                {
                    selection = _selectedBot - 1;
                    if (selection < 0)
                    {
                        selection = _numTypes - 1;
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

                PutAwayBot();
            }
            else if (_heldBot == null && _botAmount > 0 && Input.GetButtonDown(_takeOutBotButton))
            {
                buttonPressed = true;
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
                        _heldBot.transform.position = _deployTarget.position;
                        _heldBot.transform.rotation = _deployTarget.rotation;
                        _heldBot.StartMovement();
                        _heldBot = null;
                        _botAmount--;
                        _deployDelayTimer.StartTimer(_deployDelay, false);
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
        }
    }

    private void ShowValidIndicator()
    {
        _indicator.enabled = true;
        _indicator.material = _blueMaterial;

        _heldBot.transform.position = transform.position;
        _heldBot.transform.rotation = transform.rotation;
    }

    private void ShowInvalidIndicator()
    {
        _indicator.enabled = true;
        _indicator.material = _redMaterial;

        _heldBot.transform.position = transform.position;
        _heldBot.transform.rotation = transform.rotation;
    }

    private void HideIndicator()
    {
        _indicator.enabled = false;
    }
}