using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batTakeDamage : MonoBehaviour, IDamageable
{
    bool isTakingPoisonDamage;
    public void takeDamage(int dmg)
    {
        // check to make sure enemy is not already dead
        if (GetComponentInParent<NavMeshAgent>().enabled)
        {
            GetComponentInParent<batController>().HP -= dmg;

            GetComponentInParent<Animator>().SetTrigger("Damage");
            StartCoroutine(waitForDamageAnimToFinish());

            StartCoroutine(flashColor());

            if (GetComponentInParent<batController>().HP <= 0)
            {
                enemyDead();
            }
        }
    }

    public void takePoisonDamage(int damage, float poisonTime, float timeBetweenTicks)
    {
        if (!isTakingPoisonDamage)
        {
            StartCoroutine(applyPoison(damage, poisonTime, timeBetweenTicks));
        }
    }

    // poison will flash red but does not cause damage animation or stop speed
    IEnumerator applyPoison(int damage, float poisonTime, float timeBetweenTicks)
    {
        isTakingPoisonDamage = true;
        for (float i = poisonTime; i > 0;)
        {
            if (GetComponentInParent<NavMeshAgent>().enabled)
            {
                GetComponentInParent<batController>().HP -= damage;

                StartCoroutine(flashColor());

                if (GetComponentInParent<batController>().HP <= 0)
                {
                    enemyDead();
                    break;
                }
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(timeBetweenTicks);
            i -= timeBetweenTicks;
        }
        isTakingPoisonDamage = false;
    }

    void enemyDead()
    {
        gameManager.instance.enemyDecrement();
        GetComponentInParent<Animator>().SetBool("Dead", true);

        GetComponentInParent<NavMeshAgent>().enabled = false;


        //// after death, delete colliders... currently worked so takeDamage just does nothing so that enemy bodies can still be interacted with
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

    }

    IEnumerator flashColor() //changes the color of the enemy.
    {
        GetComponentInParent<batController>().rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponentInParent<batController>().rend.material.color = Color.white;
    }

    IEnumerator waitForDamageAnimToFinish()
    {
        GetComponentInParent<batController>().takingDamage = true;
        GetComponentInParent<NavMeshAgent>().velocity = Vector3.zero;
        GetComponentInParent<NavMeshAgent>().isStopped = true;
        yield return new WaitForSeconds(1);
        if (GetComponentInParent<NavMeshAgent>().enabled)
        {
            GetComponentInParent<NavMeshAgent>().isStopped = false;
            GetComponentInParent<NavMeshAgent>().speed = GetComponentInParent<batController>().chaseSpeed;
            GetComponentInParent<batController>().takingDamage = false;
        }
    }
}
