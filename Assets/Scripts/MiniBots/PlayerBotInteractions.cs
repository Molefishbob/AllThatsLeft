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
    public LayerMask _lBombableLayer = 1 << 11 | 1 << 10;
    [SerializeField]
    private string _sHackButton = "Use Object";
    [SerializeField]
    private string _sExplodeButton = "Cancel";
    public bool _bActive = false;
    private bool _bActing = false;
    private GameObject[] _goTarget = null;

    // Read inputs here
    void Update()
    {
        if (Input.GetButtonDown(_sHackButton) && _bActive && !_bActing)
        { 
            _goTarget = CheckSurroundings(_lHackableLayer, false);
            if (_goTarget != null)
            {
                GenericHackable ghOther = _goTarget[0].GetComponent<GenericHackable>();
                if (ghOther._currentStatus == GenericHackable.Status.NotHacked)
                    ghOther.TimeToStart();
            }
        }

        if (Input.GetButtonDown(_sExplodeButton) && _bActive)
        {
            _goTarget = CheckSurroundings(_lBombableLayer, true);
            if (_goTarget != null)
            {
                foreach (GameObject o in _goTarget)
                {
                    o.SetActive(false);
                }
            }
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
            _bActing = !isExplosion;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] == closestObject)
                {
                    closestIndex = i;
                }
                tmpObject[i] = hitColliders[i].gameObject;
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
}
