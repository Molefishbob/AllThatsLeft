using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployBots : MonoBehaviour, IPauseable
{
    public string _scrollButton = "Scroll";
    public string _selectBot1Button = "Bot1";
    public string _selectBot2Button = "Bot2";
    public string _selectBot3Button = "Bot3";
    //public string _selectBot4Button = "Bot4";
    //public string _selectBot5Button = "Bot5";
    public string _deployBotButton = "Deploy Bot";
    public float _buttonDelay = 0.1f;
    public Transform _attachPoint;
    public Transform _deployTarget;
    public float _extraSpaceRequired;
    public LayerMask _deployCollisionLayers;
    public LayerMask _deployableTerrain;
    public int _startingBotAmount;

    private bool _paused;
    private int _selectedBot;
    private int _numTypes = 3;
    private CharControlBase _heldBot; //TODO: change this to base bot type

    public int BotAmount
    {
        get;
        private set;
    }

    private void Awake()
    {
        BotAmount = _startingBotAmount;
    }

    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void Update()
    {
        if (!_paused)
        {
            if (_heldBot != null)
            {
                RaycastHit hit;
                if (!Physics.CheckCapsule(
                        _deployTarget.position + Vector3.up * (_heldBot.Radius + _extraSpaceRequired),
                        _deployTarget.position + Vector3.up * (_heldBot.Height - _heldBot.Radius - _extraSpaceRequired),
                        _heldBot.Radius + _extraSpaceRequired,
                        _deployCollisionLayers
                        ) &&
                    Physics.SphereCast(
                        _deployTarget.position + Vector3.up * _heldBot.Radius,
                        _heldBot.Radius,
                        Physics.gravity,
                        out hit,
                        _heldBot.SkinWidth + _deployTarget.localPosition.y,
                        _deployableTerrain))
                {
                    // TODO: make indicator blue here

                    if (Input.GetButtonDown(_deployBotButton))
                    {
                        _heldBot.transform.parent = null;
                        _heldBot.transform.position = _deployTarget.position;
                        _heldBot.transform.rotation = _deployTarget.rotation;
                        //_heldBot.activate();
                        _heldBot = null;
                        BotAmount--;
                    }
                }
                else
                {
                    // TODO: make indicator red here
                }
            }
            else
            {
                if (Input.GetButtonDown(_selectBot1Button))
                {
                    _selectedBot = 0;
                }
                else if (Input.GetButtonDown(_selectBot2Button))
                {
                    _selectedBot = 1;
                }
                else if (Input.GetButtonDown(_selectBot3Button))
                {
                    _selectedBot = 2;
                }
                /* else if (Input.GetButtonDown(_selectBot4Button))
                {
                    _selectedBot = 3;
                }
                else if (Input.GetButtonDown(_selectBot5Button))
                {
                    _selectedBot = 4;
                } */
                else if (Input.GetAxisRaw(_scrollButton) > 0.0f)
                {
                    _selectedBot++;
                    if (_selectedBot >= _numTypes)
                    {
                        _selectedBot = 0;
                    }
                }
                else if (Input.GetAxisRaw(_scrollButton) < 0.0f)
                {
                    _selectedBot--;
                    if (_selectedBot < 0)
                    {
                        _selectedBot = _numTypes - 1;
                    }
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
}