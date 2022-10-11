using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorWinConditon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //&& gameManager.instance.keyCount >= 3)
        {
            gameManager.instance.isPaused = true;
            gameManager.instance.currentMenu = gameManager.instance.levelWinMenu;
            gameManager.instance.currentMenu.SetActive(true);
            gameManager.instance.CursorLockPause();
        }
    }
}
