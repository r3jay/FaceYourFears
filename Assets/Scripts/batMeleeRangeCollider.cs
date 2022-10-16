using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class batMeleeRangeCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<batController>().targetPlayer)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(GetComponentInParent<batController>().meleeAttack());
            }
        }
    }
}
