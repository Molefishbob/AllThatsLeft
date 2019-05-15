using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineAction : BotActionBase
{
    [SerializeField]
    private string _sTrampolineButton = "Stay Action";
    private GameObject _goTrampoline;
    private BotReleaser _releaser;
    public bool _bActing = false;
    [SerializeField]
    private SingleSFXSound _assembleSound = null;
    [SerializeField]
    private SingleSFXSound _bounceSound = null;

    private SkinnedMeshRenderer[] _botRenderers = null;
    private Color[][] _cMatColors = null;
    [SerializeField]
    private Color _cBlinkColor = Color.red;
    [SerializeField]
    private float _fBlinkDelay = 0.5f;
    private float _fBlinkTimer = 0;

    protected override void Awake()
    {
        base.Awake();
        TopOfThetramp tmpTrampsHead = GetComponentInChildren<TopOfThetramp>(true);
        tmpTrampsHead._bounceSound = _bounceSound;
        _goTrampoline = tmpTrampsHead.gameObject;
        _releaser = GetComponent<BotReleaser>();

        _botRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        _cMatColors = new Color[_botRenderers.Length][];
        for (int i = 0; i < _botRenderers.Length; i++)
        {
            _cMatColors[i] = new Color[_botRenderers[i].materials.Length];
            for (int j = 0; j < _botRenderers[i].materials.Length; j++)
            {
                _cMatColors[i][j] = _botRenderers[i].materials[j].color;
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            _bPaused = true;
            return;
        }
        if (_bPaused)
        {
            _bPaused = false;
            return;
        }

        if (_bActing)
        {
            float lifeTime = _releaser.GetRemainingLifeTime();
            if (lifeTime < 5 && lifeTime > 0)
            {
                if (_fBlinkTimer >= _fBlinkDelay)
                {
                    bool wantedState = _botRenderers[0].materials[0].color != _cBlinkColor;
                    for (int i = 0; i < _botRenderers.Length; i++)
                    {
                        for (int j = 0; j < _botRenderers[i].materials.Length; j++)
                        {
                            _botRenderers[i].materials[j].color = wantedState ? _cBlinkColor : _cMatColors[i][j];
                        }
                    }
                    _fBlinkTimer = 0;
                }
                else
                {
                    _fBlinkTimer += Time.deltaTime;
                }
            }
        }

        if (!_selfMover.IsGrounded || !_bCanAct) return;

        if (Input.GetButtonDown(_sTrampolineButton))
        {
            _assembleSound.PlaySound(false);
            _goTrampoline.SetActive(true);
            _bActing = true;
            _selfMover._animator.SetTrigger("Trampoline");
            _releaser.ReleaseControls(true);
        }
    }

    public override void DisableAction()
    {
        for (int i = 0; i < _botRenderers.Length; i++)
        {
            for (int j = 0; j < _botRenderers[i].materials.Length; j++)
            {
                _botRenderers[i].materials[j].color = _cMatColors[i][j];
            }
        }
        _goTrampoline.SetActive(false);
        _bActing = false;
    }
}
