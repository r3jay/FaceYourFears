using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(dealPoisonDamage(other, GetComponentInParent<enemyAI>().timeBetweenPoisonTicks));
        }
    }
    IEnumerator dealPoisonDamage(Collider other, float timeBetweenTicks)
    {
        for (float i = GetComponentInParent<enemyAI>().poisonTime; i > 0;)
        {
            other.GetComponent<playerController>().takeDamage(GetComponentInParent<enemyAI>().poisonDamage);
            yield return new WaitForSeconds(timeBetweenTicks);
            i -= timeBetweenTicks;
        }
    }
}
