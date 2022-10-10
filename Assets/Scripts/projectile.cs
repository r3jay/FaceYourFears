using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public AudioClip projectileImpactSound;
    private GameObject hit;
    private bool collided;
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player" && !collided && col.gameObject.tag != "Muzzle")
        {
            collided = true;
            hit = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity);

            if (col.collider.GetComponent<IDamageable>() != null)
            {
                col.collider.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);
            }
        }
        if (collided == true)
        {
            Destroy(hit, 3);
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(projectileImpactSound, transform.position);
    }

}
