using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeRangeCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<enemyAI>().targetHouse)
        {
            if (other.CompareTag("Base"))
            {

                GetComponentInParent<enemyAI>().isInMeleeRange = true;
            }
        }
        else if (GetComponentInParent<enemyAI>().targetPlayer)
        {
            if (other.CompareTag("Player"))
            {

                GetComponentInParent<enemyAI>().isInMeleeRange = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (GetComponentInParent<enemyAI>().targetHouse)
        {
            if (other.CompareTag("Base"))
            {

                GetComponentInParent<enemyAI>().isInMeleeRange = false;
            }
        }
        else if (GetComponentInParent<enemyAI>().targetPlayer)
        {
            if (other.CompareTag("Player"))
            {

                GetComponentInParent<enemyAI>().isInMeleeRange = false;
            }
        }
    }
}
