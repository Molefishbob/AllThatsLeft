using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plantWind : MonoBehaviour
{
    public Animator animator;
    public string animationTriggerName = "wind";

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        animator.SetTrigger(animationTriggerName);
    }
}
