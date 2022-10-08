using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeRangeCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Base"))
        {
            GetComponentInParent<enemyAI>().isInMeleeRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Base"))
        {
            GetComponentInParent<enemyAI>().isInMeleeRange = false;
        }
    }
}
