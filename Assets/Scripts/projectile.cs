using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public AudioSource impactSound;
    //[SerializeField] AudioClip projectileImpactSound;
    [SerializeField] GameObject proImpactSound;
    //[Range(0, 1)] [SerializeField] float proImpactSoundVolume;
    private GameObject hit;
    private bool collided;
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player" && !collided && col.gameObject.tag != "Muzzle")
        {
            Instantiate(proImpactSound);
            collided = true;
            hit = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity);

            if (col.collider.GetComponent<IDamageable>() != null)
            {
                col.collider.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);
            }
        }
        if (collided == true)
        {
            Destroy(hit, 0.1f);
            Destroy(gameObject, 0.1f);
        }
    }
}
