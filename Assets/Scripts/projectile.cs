using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    private bool collided;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player" && !collided)
        {
            collided = true;
            var impact = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity) as GameObject;
            Destroy(impact, 2);
            Destroy(gameObject);

            if (col.collider.GetComponent<IDamageable>() != null)
            {
                var hit = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity) as GameObject;
                col.collider.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);
                Destroy(hit, 2);
                Destroy(gameObject);
            }
        }
    }
}
