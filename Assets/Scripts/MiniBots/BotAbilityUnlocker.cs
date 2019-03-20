using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAbilityUnlocker : MonoBehaviour
{
    [SerializeField]
    private MiniBotAbility _ability = MiniBotAbility.Hack;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.UsableMiniBotAbilities.Add(_ability);

        gameObject.SetActive(false);
    }
}