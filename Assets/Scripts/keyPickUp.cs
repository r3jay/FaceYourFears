using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class keyPickUp : MonoBehaviour
{
    public static keyPickUp instance;
    private bool playerEntered;


    private void Update()
    {
        //gameManager.instance.keyCountText.text = gameManager.instance.keyCount.ToString();
        if (Input.GetKeyDown(KeyCode.E) && playerEntered == true)
        {

            gameManager.instance.playerGetKey();
            gameManager.instance.keyCount++;
            gameManager.instance.pickUp.SetActive(false);
            gameManager.instance.pickedUp = true;
            gameManager.instance.keyIcon.SetActive(true);
            Destroy(gameObject, 0.1f);
            playerEntered = false;

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
            gameManager.instance.pickUp.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.instance.playerLeftKey();
    }
}
