using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyPickUp : MonoBehaviour
{
    private bool playerEntered;
    private int keyCount;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerEntered == true)
        {
            keyCount++;
            gameManager.instance.pickUp.SetActive(false);
            gameManager.instance.pickedUp = true;
            //gameManager.instance.keyCountText.text = keyCount.ToString("F0");
            gameManager.instance.keyIcon.SetActive(true);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player Entered Key Collider");
        if (other.CompareTag("Player") && keyCount !>= 3)
        {
            
            playerEntered = true;
            gameManager.instance.playerGetKey();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.instance.playerLeftKey();
    }
}
