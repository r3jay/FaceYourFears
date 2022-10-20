using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool targetHit = false;
        if (other.GetComponentInParent<IDamageable>() != null)
        {
            if (other.CompareTag("Base") && GetComponentInParent<enemyAI>().targetHouse)
            {
                targetHit = true;
                other.GetComponentInParent<IDamageable>().takeDamage(GetComponentInParent<enemyAI>().meleeDamage);
            }
            else if(other.CompareTag("Player") && GetComponentInParent<enemyAI>().targetPlayer)
            {
                targetHit = true;
                other.GetComponentInParent<IDamageable>().takeDamage(GetComponentInParent<enemyAI>().meleeDamage);
            }
            if(targetHit)
            {
                if (GetComponentInParent<enemyAI>())
                {
                    foreach (GameObject weapon in GetComponentInParent<enemyAI>().meleeWeapons)
                    {
                        weapon.GetComponent<Collider>().enabled = false;
                    }
                }
            }
            // Ensure that player cant be hit twice from the same attack
            
        }
    }
}
