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
