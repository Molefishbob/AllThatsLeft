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

    private List<Renderer> _botRenderers = null;
    private List<Color> _cMatColors = null;
    [SerializeField]
    private Color _cBlinkColor = Color.red;
    [SerializeField]
    private float _fBlinkDelay = 0.5f;
    private float _fBlinkTimer = 0;

    protected override void Awake()
    {
        base.Awake();
        _goTrampoline = GetComponentInChildren<TopOfThetramp>(true).gameObject;
        _releaser = GetComponent<BotReleaser>();
    }

    private void Start()
    {
        Renderer[] tmp = GetComponentsInChildren<Renderer>();
        _cMatColors = new List<Color>();
        _botRenderers = new List<Renderer>();

        for (int i = 0; i < tmp.Length; i++)
        {
            _botRenderers.Add(tmp[i]);
        }

        foreach (Renderer o in _botRenderers)
        {
            Material[] tmpMats = o.materials;
            foreach (Material p in tmpMats)
            {
                _cMatColors.Add(p.color);
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

        // Lets give em blinking!
        // Its surprisingly lightweight
        if (_bActing)
        {
            float lifeTime = _releaser.GetRemainingLifeTime();
            if (lifeTime < 5 && lifeTime > 0)
            {
                if (_fBlinkTimer >= _fBlinkDelay)
                {
                    bool wantedState = _botRenderers[0].materials[0].color != _cBlinkColor;
                    foreach (Renderer o in _botRenderers)
                    {
                        int loops = 0;
                        Material[] tmp = o.materials;
                        foreach (Material p in tmp)
                        {
                            if (wantedState)
                                p.color = _cBlinkColor;
                            else
                                p.color = _cMatColors[loops];
                            loops++;
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
            _goTrampoline.SetActive(true);
            _bActing = true;
            _selfMover._animator.SetTrigger("Trampoline");
            _releaser.ReleaseControls(true);
        }
    }

    public override void DisableAction()
    {
        foreach (Renderer o in _botRenderers)
        {
            int loops = 0;
            Material[] tmp = o.materials;
            foreach (Material p in tmp)
            {
                p.color = _cMatColors[loops];
                loops++;
            }
        }
        _goTrampoline.SetActive(false);
        _bActing = false;
    }
}
