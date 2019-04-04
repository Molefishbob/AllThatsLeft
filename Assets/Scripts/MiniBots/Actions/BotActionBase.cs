using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BotActionBase : MonoBehaviour
{
    protected BotMovement _selfMover;
    public bool _bCanAct = false;
    protected bool _bPaused;

    protected virtual void Awake()
    {
        _selfMover = GetComponent<BotMovement>();
    }

    public abstract void DisableAction();

    protected GameObject[] CheckSurroundings(LayerMask interLayer, float range)
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, range, interLayer);
        float shortestDistance = float.MaxValue;
        Collider closestObject = null;

        // Lets find the closest one
        foreach (Collider o in hitColliders)
        {
            if (shortestDistance > Vector3.Distance(o.transform.position, transform.position))
            {
                shortestDistance = Vector3.Distance(o.transform.position, transform.position);
                closestObject = o;
            }
        }

        // Lets put the closest to index 0
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
}
