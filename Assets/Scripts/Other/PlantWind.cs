using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantWind : MonoBehaviour
{
    public Animator animator;
    public string animationTriggerName = "wind";

    private void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger(animationTriggerName);
    }
}
