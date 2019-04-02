using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorMiddleHand : MonoBehaviour
{
    [SerializeField]
    private RandomSFXSound _hopsfx = null;
    private FrogEnemy _frog;
    private PatrolEnemy _scorpion;

    private void Awake()
    {
        if (transform.parent.tag == "Frog")
        {
            _frog = GetComponentInParent<FrogEnemy>();
        }
        else if (transform.parent.tag == "Scorpion")
        {
            _scorpion = GetComponentInParent<PatrolEnemy>();
        }
    }

    public void PlayHopSound()
    {
        _hopsfx.PlaySound(true);
    }

    public void AttackEnded()
    {
        if (_frog != null)
        {
            _frog.StartMoving();
        }
        if(_scorpion != null)
        {
            _scorpion.StopMoving = false;
        }
    }
}
