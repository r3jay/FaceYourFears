using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<playerController>().isTakingPoisonDamage)
            {
                StartCoroutine(dealPoisonDamage(other, GetComponentInParent<enemyAI>().timeBetweenPoisonTicks, GetComponentInParent<enemyAI>().poisonDamage));
            }
        }
    }
    IEnumerator dealPoisonDamage(Collider other, float timeBetweenTicks, int damage)
    {
        other.GetComponent<playerController>().isTakingPoisonDamage = true;
        for (float i = GetComponentInParent<enemyAI>().poisonTime; i > 0;)
        {
            other.GetComponent<playerController>().takeDamage(damage);
            yield return new WaitForSeconds(timeBetweenTicks);
            i -= timeBetweenTicks;
        }
        other.GetComponent<playerController>().isTakingPoisonDamage = false;
    }
}
