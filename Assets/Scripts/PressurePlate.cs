using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IButtonInteraction
{
    private const int PlayerLayer = 10;
    [SerializeField, Tooltip("Add the object that is affected by the button being pressed.\nMust Implement IButtonIntercation -Interface!")]
    private GameObject _target;
    private IButtonInteraction _tInt;
    private BoxCollider _coll;
    private bool _isButtonPressed;

    /// <summary>
    /// Awake is called before the first frame update
    /// </summary>
    void Awake()
    {
        _coll = GetComponent<BoxCollider>();
        if (_target.GetComponent<IButtonInteraction>() == null)
        {
            Debug.LogError("The Target of " + gameObject.name + " " + transform.position + " HAS to implement IButtonInteraction");
        } else
        {
            _tInt = _target.GetComponent<IButtonInteraction>();
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// What happens when the button is pressed down
    /// </summary>
    public void ButtonDown()
    {
        /// TODO: ADD ANIMATION
        _tInt.ButtonDown();
    }

    /// <summary>
    /// What happens when the button is no longer pressed down
    /// </summary>
    public void ButtonUp()
    {
        /// TODO: ADD ANIMATION
        _tInt.ButtonUp();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == PlayerLayer)
        {
            if (!_isButtonPressed)
            {
                Debug.Log("COLLISION STAY");
                ButtonDown();
                _isButtonPressed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PlayerLayer)
        {
            if (_isButtonPressed)
            {
                Debug.Log("COLLISION EXIT");
                ButtonUp();
                _isButtonPressed = false;
            }
        }
    }
}
