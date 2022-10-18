using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject impactEffect;

    [Header("----- Bullet Stats -----")]
    public int damage;
    public int speed;
    public int destroyTime;  //this is for clean up, so the frames arent falling. taking up too much memory.

    [Header("----- Status Effects -----")]
    [SerializeField] bool projectileStuns;
    [SerializeField] float stunTime;
    [SerializeField] bool projectileSlows;
    [SerializeField] float slowTime;
    [Range(0, 1)] [SerializeField] float slowModifier;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed; // sends the bullet toward the transforms forward direction. 
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            if (projectileStuns)
            {
                if (other.GetComponent<playerController>())
                {
                    other.GetComponent<playerController>().stunStatusEffectActive = true;
                    other.GetComponent<playerController>().stunTime = stunTime;
                }
            }
            if (projectileSlows)
            {
                if (other.GetComponent<playerController>())
                {
                    other.GetComponent<playerController>().slowStatusEffectActive = true;
                    other.GetComponent<playerController>().slowTime = slowTime;
                    other.GetComponent<playerController>().selfSlowModifier = slowModifier;
                }
            }
            if (damage > 0)
            {
                other.GetComponent<IDamageable>().takeDamage(damage);
            }
            if (impactEffect)
            {
                Instantiate(impactEffect, other.transform.localPosition, other.transform.localRotation);
            }
            Destroy(gameObject);
        }

    }

}
