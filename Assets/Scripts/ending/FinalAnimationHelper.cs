using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAnimationHelper : MonoBehaviour
{

    public Animator mainCharAnimator;
    public string runBoolName = "Run";
    public bool run = false;

    public Animator creditsAnimator;
    public string creditsBoolName = "credits";
    public bool showCredits = false;

    public void StartRun() {
        run = true;
        mainCharAnimator.SetBool(runBoolName, run);
    }

    public void StartCredits() {
        showCredits = true;
        creditsAnimator.SetBool(creditsBoolName, showCredits);
    }

    public void AnimEvent() {
        Debug.Log("I am anim event");
    }
}
