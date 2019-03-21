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
    private int _iReleaseDelay = 2;
    private int _iReleaseDelayCounter = 0;
    private GameObject[] _goTarget = null;
    private PlayerMovement _selfMover;

    void Awake()
    {
        _selfMover = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetButtonDown(_sHackButton) && _bActive && !_bActing)
        {
            _goTarget = CheckSurroundings(_lHackableLayer, false);
            if (_goTarget != null)
            {
                GenericHackable ghOther = _goTarget[0].GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                {
                    _bActing = true;
                    ghOther.TimeToStart();
                    ReleaseControls();
                }
            }
        }

        if (Input.GetButtonDown(_sExplodeButton) && _bActive)
        {
            _goTarget = CheckSurroundings(_lBombableLayer, true);
            if (_goTarget != null)
            {
                foreach (GameObject o in _goTarget)
                {
                    o.gameObject.GetComponent<IDamageReceiver>()?.TakeDamage(1);
                }
                ReleaseControls();
            }
        }

        if (_bReleasing)
        {
            if (_iReleaseDelayCounter >= _iReleaseDelay)
            {
                GameManager.Instance.Player.ControlsDisabled = !_bReleasing;
                _iReleaseDelayCounter = 0;
                _bReleasing = !_bReleasing;
            }
            _iReleaseDelayCounter++;
        } 

        if (Input.GetButtonDown(_sStayButton) && _bActive)
            ReleaseControls();
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
    public void ReleaseControls()
    {
        _selfMover.ControlsDisabled = true;
        _bReleasing = true;
        GameManager.Instance.Camera.GetNewTarget(GameManager.Instance.Player.transform);
    }

    public void StopActing()
    {
        if (_bActing)
        {
            _bActing = false;
            Debug.Log(_goTarget[0].GetComponent<GenericHackable>());
            _goTarget[0].GetComponent<GenericHackable>()?.TimeToLeave();
            _goTarget = null;
        }
    }
}
