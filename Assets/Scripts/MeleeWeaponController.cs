using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {

            other.GetComponent<IDamageable>().takeDamage(GetComponentInParent<enemyAI>().meleeDamage);
            // Ensure that player cant be hit twice from the same attack
            if (GetComponentInParent<enemyAI>())
            {
                foreach (GameObject weapon in GetComponentInParent<enemyAI>().meleeWeapons)
                {
                    weapon.GetComponent<Collider>().enabled = false;
                }
            }
        }
    }
}
