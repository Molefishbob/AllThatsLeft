using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBotInteractions : MonoBehaviour
{
    [SerializeField]
    private float _fDetectRadius = 2;
    [SerializeField]
    private float _fExplodeRadius = 4;
    public LayerMask _lHackableLayer = 1 << 18;
    public LayerMask _lBombableLayer = 1 << 11 | 1 << 10 | 1 << 9;
    [SerializeField]
    private string _sHackButton = "Hack Action";
    [SerializeField]
    private string _sExplodeButton = "Bomb Action";
    [SerializeField]
    private string _sStayButton = "Stay Action";
    public bool _bActive
    {
        get { return _bactive; }
        set
        {
            _bactive = value;
            _selfMover.ControlsDisabled = !value;
            _selfMover.SetControllerActive(value);
        }
    }
    private bool _bactive = false;
        [SerializeField]
    private bool _bActing = false;
    private bool _bReleasing = false;
    [SerializeField]
    private float _fReleaseDelay = 2;
    private ScaledOneShotTimer _ostRelease;
    private GameObject[] _goTarget = null;
    private PlayerMovement _selfMover;

    void Awake()
    {
        _selfMover = GetComponent<PlayerMovement>();
        _ostRelease = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        _ostRelease.OnTimerCompleted += ActualRelease;
    }

    private void OnDestroy()
    {
        if (_ostRelease != null)
        {
            _ostRelease.OnTimerCompleted -= ActualRelease;
        }
    }

    void Update()
    {
        // Hack
        if (Input.GetButtonDown(_sHackButton) && _bActive && !_bActing && !_bReleasing)
        {
            _goTarget = CheckSurroundings(_lHackableLayer, false);
            if (_goTarget != null)
            {
                GenericHackable ghOther = _goTarget[0].GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                {
                    _bActing = true;
                    ghOther.TimeToStart();
                    ReleaseControls(true);
                }
            }
        }

        // Explode
        if (Input.GetButtonDown(_sExplodeButton) && _bActive && !_bReleasing)
        {
            _goTarget = CheckSurroundings(_lBombableLayer, true);
            if (_goTarget != null)
            {
                // TODO fix this
                // The bot will "charge" for a while
                foreach (GameObject o in _goTarget)
                {
                    if (o != gameObject)
                        o.GetComponent<IDamageReceiver>()?.TakeDamage(1);
                }
                ReleaseControls(true);
            }
        }

        // Just release
        if (Input.GetButtonDown(_sStayButton) && _bActive)
        {
            _ostRelease.StopTimer();
            ReleaseControls(false);
        }
    }

    private GameObject[] CheckSurroundings(LayerMask interLayer, bool isExplosion)
    {
        Collider[] hitColliders;
        if (!isExplosion)
        {
            hitColliders = Physics.OverlapSphere(transform.position, _fDetectRadius, interLayer);
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
                Debug.DrawLine(transform.position, tmpObject[i].transform.position,Color.red, 2f);
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

    // Release the controls back to the player
    public void ReleaseControls(bool withDelay)
    {
        if (withDelay)
        {
            _selfMover.ControlsDisabled = true;
            _bReleasing = true;
            _ostRelease.StartTimer(_fReleaseDelay);
        }
        else
        {
            _selfMover.ControlsDisabled = true;
            _bReleasing = true;
            ActualRelease();
        }
    }

    public void StopActing()
    {
        if (_bActing)
        {
            _bActing = false;
            _goTarget[0].GetComponent<GenericHackable>()?.TimeToLeave();
            _goTarget = null;
        }
    }

    private void ActualRelease()
    {
        GameManager.Instance.Player.ControlsDisabled = !_bReleasing;
        _bactive = false;
        _bReleasing = !_bReleasing;
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform);
    }
}
