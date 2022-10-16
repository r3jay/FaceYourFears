using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lichHandsController : MonoBehaviour, IDamageable
{
    [SerializeField] Animator animator;
    [SerializeField] Renderer rend;

    [SerializeField] int HP;
    [SerializeField] float destroyTime;
    [SerializeField] float stunTime;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
    private void OnDestroy()
    {
        if (gameManager.instance.playerController != null)
        {
            gameManager.instance.playerController.stunTime = 0;
            gameManager.instance.playerController.stunStatusEffectActive = false;
        }
    }
    public void takeDamage(int dmg)
    {
        HP -= dmg;
        animator.SetTrigger("Damage");

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            enemyDead();
        }
    }
    IEnumerator flashColor() //changes the color of the enemy.
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }
    void enemyDead()
    {
        animator.SetBool("Dead", true);
        //// after death, delete colliders... currently worked so takeDamage just does nothing so that enemy bodies can still be interacted with
        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!gameManager.instance.playerController.stunStatusEffectActive)
            {
                gameManager.instance.playerController.stunTime = stunTime;
                gameManager.instance.playerController.stunStatusEffectActive = true;
            }
        }
    }
}
