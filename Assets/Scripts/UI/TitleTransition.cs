using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTransition : MonoBehaviour
{
    public MenuController _controller;

    public void EnableMainMenuPanel()
    {
        transform.localScale = Vector3.one;
        _controller.EnableMainMenuPanel();
        this.enabled = false;
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            GetComponent<Animator>().SetTrigger("StopAnimation");
            EnableMainMenuPanel();
        }
    }
}
