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