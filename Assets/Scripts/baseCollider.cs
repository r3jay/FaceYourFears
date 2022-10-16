using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            if (GetComponentInParent<enemyAI>())
            {
                GetComponentInParent<enemyAI>().houseInRange = true;
                if (GetComponentInParent<enemyAI>().isTreant)
                {
                    GetComponentInParent<enemyAI>().isMeleeAttacker = true;
                    GetComponentInParent<enemyAI>().shootRate = 2;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Base"))
        {
            GetComponentInParent<enemyAI>().houseInRange = false;
        }
    }
}
