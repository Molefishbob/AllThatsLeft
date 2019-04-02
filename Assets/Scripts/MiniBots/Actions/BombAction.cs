using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : BotActionBase
{
    [SerializeField]
    private string _sExplodeButton = "Bomb Action";
    public LayerMask BombableLayer { get { return _lBombableLayer; } }
    [SerializeField]
    private LayerMask _lBombableLayer = 1 << 11 | 1 << 10 | 1 << 9;
    [SerializeField]
    private float _fExplodeRadius = 4;
    public ParticleSystem[] ExplosionParticles { get { return _psExplosion; } }
    private ParticleSystem[] _psExplosion = null;
    private bool _bFirstEnable = true;
    [SerializeField]
    private GameObject _goParticlePrefab = null;
    private GameObject _goParticleHolder;
    private GameObject[] _goTarget;
    private Projector _shadowProjector = null;
    private BotReleaser _releaser = null;

    void OnDrawGizmosEnabled()
    {
        Gizmos.DrawWireSphere(transform.position, _fExplodeRadius);
    }

    void OnEnable()
    {
        if (!_bFirstEnable && _psExplosion != null)
        {
            foreach (ParticleSystem o in _psExplosion)
            {
                o.gameObject.SetActive(false);
            }
        }
        else
        {
            _bFirstEnable = false;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _shadowProjector = GetComponentInChildren<Projector>(true);
        _releaser = GetComponent<BotReleaser>();
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

        if (!_selfMover.IsGrounded || !_bCanAct) return;

        if (Input.GetButtonDown(_sExplodeButton))
        {
            _selfMover._animator.SetBool("Explode", true);
            if (_psExplosion == null)
            {
                _goParticleHolder = Instantiate(_goParticlePrefab, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
                _psExplosion = _goParticleHolder.GetComponentsInChildren<ParticleSystem>(true);
            }
            else
            {
                foreach (ParticleSystem o in _psExplosion)
                {
                    o.gameObject.SetActive(true);
                }
            }
            _releaser.ReleaseControls(true);
        }
    }

    public override void DisableAction()
    {
        if (_psExplosion != null)
        {
            _goParticleHolder.transform.parent = transform;
            _goParticleHolder.transform.localPosition = Vector3.zero;
        }
        _shadowProjector.enabled = true;
    }

    public void ExplodeBot()
    {
        _goTarget = CheckSurroundings(_lBombableLayer, _fExplodeRadius);
        if (_goTarget != null)
        {
            foreach (GameObject o in _goTarget)
            {
                if (o != gameObject)
                    o.GetComponent<IDamageReceiver>()?.TakeDamage(1);
            }
        }
        _goParticleHolder.transform.parent = null;
        _shadowProjector.enabled = false;
    }
}
