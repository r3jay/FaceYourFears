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
            GetComponent<Collider>().enabled = false;
        }
    }
}
