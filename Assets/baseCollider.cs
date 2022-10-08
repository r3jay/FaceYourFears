using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("House collided");
            other.GetComponentInParent<enemyAI>().houseInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<enemyAI>().houseInRange = false;
        }
    }
}
