using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip projectileImpactSound;
    [Range(0, 1)] [SerializeField] float proImpactSoundVolume;

    private bool collided;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player" && !collided && col.gameObject.tag != "Muzzle")
        {
            collided = true;
            var impact = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity) as GameObject;
            aud.PlayOneShot(projectileImpactSound, proImpactSoundVolume);
            Destroy(impact, 2);
            Destroy(gameObject);

            if (col.collider.GetComponent<IDamageable>() != null)
            {
                var hit = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity) as GameObject;
                col.collider.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);
                aud.PlayOneShot(projectileImpactSound, proImpactSoundVolume);
                Destroy(hit, 2);
                Destroy(gameObject);
            }
        }
    }
}
