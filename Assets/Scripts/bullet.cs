using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    public int damage;
    public int speed;
    public int destroyTime;  //this is for clean up, so the frames arent falling. taking up too much memory.

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed; // sends the bullet toward the transforms forward direction. 
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null && other.CompareTag("Player") == true)
        {
            other.GetComponent<IDamageable>().takeDamage(damage);
        }
        Destroy(gameObject);

    }
}
