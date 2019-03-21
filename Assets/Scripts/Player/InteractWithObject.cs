using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            if (Input.GetButtonDown("Fire3"))
            {
                Debug.Log("You are interacting with " + hit.collider.gameObject.name);
            }
        }
    }
}
