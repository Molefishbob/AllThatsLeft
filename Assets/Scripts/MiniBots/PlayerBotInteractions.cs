using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBotInteractions : MonoBehaviour
{
    [SerializeField]
    private float _fExplodeRadius = 4.0f;
    public LayerMask _lHackableLayer = 1 << 18;
    public LayerMask _lBombableLayer = 1 << 11 | 1 << 10 | 1 << 9;
    [SerializeField]
    private string _sHackButton = "Hack Action";
    [SerializeField]
    private string _sExplodeButton = "Bomb Action";
    [SerializeField]
    private string _sTrampolineButton = "Stay Action";
    [SerializeField]
    private GameObject _goParticlePrefab = null;
    [SerializeField]
    private GameObject _goTrampoline = null;
    private GameObject _goParticleHolder;
    private ParticleSystem[] _psExplosion = null;
    public bool _bActive
    {
        get { return _bactive; }
        set
        {
            _bactive = value;
            _selfMover.ControlsDisabled = !value;
            _selfMover.SetControllerActive(true);
        }
    }
    private bool _bactive = false;
    [SerializeField]
    private bool _bHacking = false;
    private bool _bFirstEnable = true;
    [SerializeField]
    private float _fReleaseDelay = 2.0f;
    [SerializeField]
    private float _transitionTime = 1.0f;
    [SerializeField]
    private float _fLifeTime = 30.0f;
    private ScaledOneShotTimer _ostRelease;
    private ScaledOneShotTimer _ostDisable;
    private PhysicsOneShotTimer _ostLife;
    private GameObject[] _goTarget = null;
    private BotMovement _selfMover;
    private Projector _shadowProjector;

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

    void Awake()
    {
        _selfMover = GetComponent<BotMovement>();
        _ostRelease = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostDisable = gameObject.AddComponent<ScaledOneShotTimer>();
        _ostLife = gameObject.AddComponent<PhysicsOneShotTimer>();
        _shadowProjector = gameObject.GetComponentInChildren<Projector>();
    }

    private void Start()
    {
        _ostRelease.OnTimerCompleted += ActualRelease;
        _ostDisable.OnTimerCompleted += DisableSelf;
        _ostLife.OnTimerCompleted += _selfMover.Die;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _fExplodeRadius);
    }

    private void OnDestroy()
    {
        if (_ostRelease != null)
        {
            _ostRelease.OnTimerCompleted -= ActualRelease;
        }
        if (_ostDisable != null)
        {
            _ostDisable.OnTimerCompleted -= DisableSelf;
        }
        if (_ostLife != null && _selfMover != null)
        {
            _ostLife.OnTimerCompleted -= _selfMover.Die;
        }
    }

    void Update()
    {
        if (GameManager.Instance.GamePaused) return;
        if (!_selfMover.IsGrounded || _selfMover.ControlsDisabled) return;

        // Hack
        if (Input.GetButtonDown(_sHackButton))
        {
            _goTarget = CheckSurroundings(_lHackableLayer, false);
            if (_goTarget != null)
            {
                GenericHackable ghOther = _goTarget[0].GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                {
                    _bHacking = true;
                    ghOther.TimeToStart();
                    ReleaseControls(true);
                }
            }
        }

        // Explode
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
            ReleaseControls(true);
        }

        // Trampoline
        if (Input.GetButtonDown(_sTrampolineButton))
        {
            _goTrampoline.SetActive(true);
            _ostRelease.StopTimer();
            _ostLife.StartTimer(_fLifeTime);
            ReleaseControls(false);
        }
    }

    /// <summary>
    /// This will explode the bot and send tell killable things to die
    /// Maybe called only from animator
    /// </summary>
    public void ExplodeBot()
    {
        _goTarget = CheckSurroundings(_lBombableLayer, true);
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

    private GameObject[] CheckSurroundings(LayerMask interLayer, bool isExplosion)
    {
        Collider[] hitColliders;
        if (!isExplosion)
        {
            Vector3 capsuleCenter = transform.position + _selfMover._controller.center;
            Vector3 halfHeight = Vector3.up * ((_selfMover._controller.height / 2.0f) - _selfMover._controller.radius);
            hitColliders = Physics.OverlapCapsule(capsuleCenter - halfHeight, capsuleCenter + halfHeight, _selfMover._controller.radius, interLayer);
        }
        else
        {
            hitColliders = Physics.OverlapSphere(transform.position, _fExplodeRadius, interLayer);
        }
        float shortestDistance = float.MaxValue;
        Collider closestObject = null;
        foreach (Collider o in hitColliders)
        {
            if (shortestDistance > Vector3.Distance(o.transform.position, transform.position))
            {
                shortestDistance = Vector3.Distance(o.transform.position, transform.position);
                closestObject = o;
            }
        }


        if (closestObject != null)
        {
            int closestIndex = 0;
            GameObject[] tmpObject = new GameObject[hitColliders.Length];
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] == closestObject)
                {
                    closestIndex = i;
                }
                tmpObject[i] = hitColliders[i].gameObject;
                Debug.DrawLine(transform.position, tmpObject[i].transform.position, Color.red, 2f);
            }
            if (closestIndex != 0)
            {
                tmpObject[closestIndex] = tmpObject[0];
                tmpObject[0] = closestObject.gameObject;
            }
            return tmpObject;
        }
        return null;
    }
    public void OnDisable()
    {
        _bHacking = false;
        _ostDisable.StopTimer();
        _ostRelease.StopTimer();
        _ostLife.StopTimer();
        _selfMover._animator.SetBool("Explode", false);
        _shadowProjector.enabled = true;
        _selfMover.SetControllerActive(false);
    }

    // Release the controls back to the player
    public void ReleaseControls(bool withDelay)
    {
        _selfMover.ControlsDisabled = true;
        GameObject[] hacks = CheckSurroundings(_lHackableLayer, false);
        if (hacks != null && hacks.Length > 0)
        {
            foreach (GameObject item in hacks)
            {
                GenericHackable hack = item.GetComponent<GenericHackable>();
                hack?.ShowPrompt(false);
            }
        }
        if (withDelay)
        {
            _ostRelease.StartTimer(_fReleaseDelay);
        }
        else
        {
            ActualRelease();
        }
    }

    public void StopActing()
    {
        if (_bHacking)
        {
            _bHacking = false;
            _goTarget[0].GetComponent<GenericHackable>()?.TimeToLeave();
            _goTarget = null;
        }
        gameObject.SetActive(false);
    }

    private void ActualRelease()
    {
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform, _transitionTime, true);
        _ostDisable.StartTimer(_transitionTime);
    }

    private void DisableSelf()
    {
        GameManager.Instance.Player.ControlsDisabled = false;
        if (!_bHacking && !_ostLife.IsRunning)
        {
            if (_psExplosion != null)
            {
                _goParticleHolder.transform.parent = transform;
                _goParticleHolder.transform.localPosition = Vector3.zero;
            }
            _goTrampoline.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
