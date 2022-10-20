using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public AudioClip projectileImpactSound;
    private GameObject hit;
    private bool collided;
    public bool isAoe;
    List<Collider> aoeCol;

    void Start()
    {
        //Instantiate(proList[selectedPro].projectile, shootPos.transform.position, shootPos.transform.rotation);
        //rb.velocity = Camera.main.transform.forward * gameManager.instance.playerController.proSpeed;
        rb.velocity = gameManager.instance.playerController.destination.normalized * gameManager.instance.playerController.proSpeed;

        aoeCol = new List<Collider>();
        Destroy(gameObject, gameManager.instance.playerController.destroyTime);
    }
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag != "Bullet" && col.gameObject.tag != "Player" && !collided && col.gameObject.tag != "Muzzle")
        {
            collided = true;
            hit = Instantiate(gameManager.instance.playerController.impactE, col.contacts[0].point, Quaternion.identity);
            isAoe = gameManager.instance.playerController.isAoe;


            if (col.collider.GetComponent<IDamageable>() != null)
            {
                col.collider.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);

                if (gameManager.instance.playerController.stun == true)
                {
                    if (col.collider.GetComponent<enemyAI>())
                    {
                        col.collider.GetComponent<enemyAI>().stunStatusEffectActive = true;
                        col.collider.GetComponent<enemyAI>().stunTime = gameManager.instance.playerController.statusEffectTime_stun;
                    }
                }
                if (gameManager.instance.playerController.slowDown > 0)
                {
                    if (col.collider.GetComponent<enemyAI>())
                    {
                        col.collider.GetComponent<enemyAI>().slowStatusEffectActive = true;
                        col.collider.GetComponent<enemyAI>().slowTime = gameManager.instance.playerController.statusEffectTime_slow;
                        col.collider.GetComponent<enemyAI>().slowModifier = gameManager.instance.playerController.slowDown;
                    }
                }
                if (gameManager.instance.playerController.DOTdamage > 0)
                {
                    if (col.collider.GetComponent<enemyAI>())
                    {
                        col.collider.GetComponent<enemyAI>().takePoisonDamage(gameManager.instance.playerController.DOTdamage, gameManager.instance.playerController.DOTtime, gameManager.instance.playerController.timeBetweenTicks);
                    }
                }
            }
        }
        if (collided == true)
        {
            if (Physics.OverlapSphere(transform.position, gameManager.instance.playerController.aoeRadius).Length > 0)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, gameManager.instance.playerController.aoeRadius);

                foreach (Collider c in colliders)
                {
                    //if (!c.isTrigger && c != this.GetComponent<Collider>())
                    //if (c.GetComponent<IDamageable>() != null && !c.isTrigger )
                    if (c.GetComponent<IDamageable>() != null && !c.isTrigger && c != this.GetComponent<Collider>() && c.GetComponent<enemyAI>())
                    {
                        aoeCol.Add(c);
                    }
                }
            }

            Destroy(hit, 3);
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(projectileImpactSound, transform.position);
        if (isAoe == true)
        {
            if (aoeCol != null)
            {
                foreach (Collider c in aoeCol)
                {
                    if (c.GetComponent<IDamageable>() != null)
                    {
                        c.GetComponent<IDamageable>().takeDamage(gameManager.instance.playerController.playerDamage);
                    }
                }
            }

        }
    }
}
//[Header("----- Components -----")]
//[SerializeField] Rigidbody rb;

//[Header("----- Bullet Stats -----")]
//public int damage;
//public int speed;
//public int destroyTime;  //this is for clean up, so the frames arent falling. taking up too much memory.

//// Start is called before the first frame update
//void Start()
//{
//    rb.velocity = transform.forward * speed; // sends the bullet toward the transforms forward direction. 
//    Destroy(gameObject, destroyTime);
//}

//private void OnTriggerEnter(Collider other)
//{
//    if (other.GetComponent<IDamageable>() != null)
//    {
//        other.GetComponent<IDamageable>().takeDamage(damage);
//        Destroy(gameObject);
//    }

//}