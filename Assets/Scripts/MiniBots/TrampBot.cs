using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampBot : GenericBot
{
    public float _fBoostForce = 10;
    [SerializeField, Tooltip("Has no function, only shows number, which is not accurate lol")]
    private float _fDistanceTraveled;
    public float _fJumpDuration = 1;
    private float jumpTimer;
    const int _iPlayerLayer = 10;
    private CharControlBase _PlayerMover;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_PlayerMover != null){
            jumpTimer += Time.deltaTime;
            Vector3 tmp = Vector3.zero;
            float reminder = jumpTimer / _fJumpDuration;
            tmp.y += _fBoostForce * (1 - reminder);
            tmp = Vector3.Lerp(Vector3.zero, tmp, jumpTimer);
            _fDistanceTraveled += tmp.y;
            _PlayerMover.AddDirectMovement(tmp);
            if (jumpTimer > _fJumpDuration){
                _PlayerMover = null;
                jumpTimer = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _iPlayerLayer){
            _PlayerMover = other.GetComponent<CharControlBase>();
            _fDistanceTraveled = 0;
        }
    }
}
