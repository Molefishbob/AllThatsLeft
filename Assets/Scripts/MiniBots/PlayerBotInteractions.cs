using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBotInteractions : MonoBehaviour
{
    [SerializeField]
    private float _fDetectRadius = 2;
    public LayerMask _lHackableLayer = 1 << 18;
    [SerializeField]
    private string _sHackButton = "Use Object";
    [SerializeField]
    private string _sExplodeButton = "Cancel";
    public bool _bActive = false;
    private bool _bActing = false;
    private GameObject _goTarget = null;

    // Read inputs here
    void Update()
    {
        if (Input.GetButtonDown(_sHackButton) && _bActive && !_bActing)
        { 
            _goTarget = CheckSurroundings(_lHackableLayer);
            if (_goTarget != null)
            {
                GenericHackable ghOther = _goTarget.GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                    ghOther.TimeToStart();
            }
        }
    }

    private GameObject CheckSurroundings(LayerMask interLayer)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _fDetectRadius, interLayer.value);
        float shortestDistance = float.MaxValue;
        Collider closestObject = null;
        foreach (Collider o in hitColliders)
        {
            int tmp = 1 << o.gameObject.layer;
            if (shortestDistance > Vector3.Distance(o.transform.position, transform.position) && tmp == interLayer.value)
            {
                shortestDistance = Vector3.Distance(o.transform.position, transform.position);
                closestObject = o;
            }
        }
        if (closestObject != null)
        {
            _bActing = true;
            return closestObject.gameObject;
        }
        return null;
    }
}
