using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployBots : MonoBehaviour, IPauseable
{
    public string _scrollButton = "Scroll";
    public string _selectBot1Button = "Bot1";
    public string _selectBot2Button = "Bot2";
    public string _selectBot3Button = "Bot3";
    public string _deployBotButton = "Deploy Bot";
    public Transform _deployTarget;
    public float _extraSpaceRequired;
    public LayerMask _deployCollisionLayers;
    public LayerMask _deployableTerrain;

    private bool _paused;
    private int _selectedBot;
    private CharControlBase _heldBot; //TODO: change this to base bot type

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
                        //_heldBot.activate
                        _heldBot = null;
                    }
                }
                else
                {
                    // TODO: make indicator red here
                }
            }
            else
            {
                // TODO: hide indicator
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