using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batMeleeWeaponController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {

            other.GetComponent<IDamageable>().takeDamage(GetComponentInParent<batController>().meleeDamage);
            // Ensure that player cant be hit twice from the same attack
            GetComponent<Collider>().enabled = false;
        }
    }
}
