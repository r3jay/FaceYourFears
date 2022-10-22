using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treantSeed : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float upwardArc;
    [SerializeField] GameObject seedSpawnObject;
    [SerializeField] float minXOffset, maxXOffset;

    [SerializeField] GameObject impactEffect;

    private void Start()
    {
        Vector3 velocity = new Vector3(Random.Range(minXOffset,maxXOffset), upwardArc, 0);
        velocity += transform.forward * speed;
        rb.velocity = velocity; // sends the bullet toward the transforms forward direction. 

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
        {
            Instantiate(seedSpawnObject, transform.localPosition, transform.localRotation);
            gameManager.instance.enemyIncrement(1);
        }
        Instantiate(impactEffect, transform.localPosition, transform.localRotation);
        Destroy(gameObject);
    }
}
