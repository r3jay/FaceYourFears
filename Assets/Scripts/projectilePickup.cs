using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectilePickup : MonoBehaviour
{
    [SerializeField] projectileStats proStat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerController.projectilePickup(proStat);
            Destroy(gameObject);
        }
    }
}

//public class projectileshooter : MonoBehaviour
//{
//    public Camera cam;
//    public GameObject projectile;
//    public Transform leftHandShot, rightHandshot;
//    public float projectileSpeed = 30;
//    public float fireRate = 4;

//    private Vector3 destination;
//    private bool currentHand;
//    private float fireDelay;

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetButton("Fire1") && Time.time >= fireDelay)
//        {
//            fireDelay = Time.time + 1 / fireRate;
//            shootProjectile();
//        }
//    }
//    void shootProjectile()
//    {
//        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit))
//            destination = hit.point;
//        else
//            destination = ray.GetPoint(1000);
//        if (currentHand)
//        {
//            currentHand = false;
//            instantiateProjectile(leftHandShot);
//        }
//        else
//        {
//            currentHand = true;
//            instantiateProjectile(rightHandshot);
//        }
//    }
//    void instantiateProjectile(Transform shotPoint)
//    {
//        var projectileObj = Instantiate(projectile, shotPoint.position, Quaternion.identity) as GameObject;
//        projectileObj.GetComponent<Rigidbody>().velocity = (destination - shotPoint.position).normalized * projectileSpeed;
//    }
//}
