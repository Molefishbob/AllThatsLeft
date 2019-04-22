using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideCursor : MonoBehaviour
{
    [SerializeField]
    private string _mouseX = "Mouse X";
    [SerializeField]
    private string _mouseY = "Mouse Y";
    [SerializeField]
    private string _horizontal = "Horizontal";
    [SerializeField]
    private string _vertical = "Vertical";
    [SerializeField]
    private string _submit = "Submit";
    [SerializeField]
    private string _cancel = "Cancel";

    private void OnEnable()
    {
        GameManager.Instance.ShowCursor = false;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.ShowCursor = false;
    }

    private void Update()
    {
        if (Input.GetAxisRaw(_mouseX) != 0.0f ||
            Input.GetAxisRaw(_mouseY) != 0.0f)
        {
            GameManager.Instance.ShowCursor = true;
        }
        else if (Input.GetAxisRaw(_horizontal) != 0.0f ||
                 Input.GetAxisRaw(_vertical) != 0.0f ||
                 Input.GetButtonDown(_submit) ||
                 Input.GetButtonDown(_cancel))
        {
            GameManager.Instance.ShowCursor = false;
        }
    }
}