using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointPole : MonoBehaviour
{
    public int id;
    public Transform SpawnPoint;

    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private string _triggerName = "getChecked";
    [SerializeField]
    private SingleSFXSound _sound = null;
    [SerializeField]
    private SkinnedMeshRenderer _renderer = null;
    [SerializeField]
    private Material _checkedMaterial = null;
    [SerializeField]
    private Transform _pole = null;
    [SerializeField, Range(-180, 180)]
    private float _windAngle = 0;

    private void Start()
    {
        if (_pole != null) _pole.rotation = Quaternion.Euler(0, _windAngle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.LevelManager.SetCheckpoint(this))
        {
            _animator.SetTrigger(_triggerName);
            _sound.PlaySound(false);
            Checked();
        }
    }

    public void Checked()
    {
        if (_renderer != null) _renderer.material = _checkedMaterial;
    }
}
