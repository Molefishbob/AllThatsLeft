using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IButtonInteraction
{
    [Tooltip("The name of the 'opening' trigger parameter in the animator")]
    public string _openBool = "Open";
    private Animator _anim;
    [Tooltip("The duration after which the symbol goes off")]
    public float _delayDuration = 0.4f;
    [SerializeField]
    protected GameObject _symbol = null;
    [SerializeField]
    protected SingleSFXSound _openSound = null;
    protected ScaledOneShotTimer _delayTimer;

    // Awake is called before the first frame update
    void Awake()
    {
        _anim = GetComponent<Animator>();
        _delayTimer = gameObject.AddComponent<ScaledOneShotTimer>();
    }

    private void Start()
    {
        _delayTimer.OnTimerCompleted += SymbolDown;
    }

    public void ButtonDown()
    {
        _delayTimer.StartTimer(_delayDuration);
        _anim.SetBool(_openBool,true);
        _openSound.PlaySound();
    }

    public void ButtonUp()
    {
        _anim.SetBool(_openBool,false);
        _openSound.PlaySound();
    }
    
    protected virtual void SymbolDown()
    {
        try
        {
            _symbol.SetActive(false);
        }
        catch
        {
            Debug.LogError(gameObject.name + " has to have a symbol!");
        }
    }
    private void OnDestroy()
    {
        if (_delayTimer != null)
            _delayTimer.OnTimerCompleted -= SymbolDown;
    }
}
