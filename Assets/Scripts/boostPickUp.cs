using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boostPickUp : MonoBehaviour
{
    [SerializeField] boostStats boostStat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerController.boostPickUp(boostStat);
            Destroy(gameObject);
        }
    }
}
