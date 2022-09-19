using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerController;
    public GameObject playerSpawnPosition;

    public GameObject currentMenu;

    public GameObject reticle;

    public GameObject pauseMenu;
    public GameObject playerDamage;
    public GameObject playerWinMenu;
    public GameObject playerDeadMenu;

    public Image HPBar;
    public TextMeshProUGUI remainingEnemiesLabel;

    public int enemyCount;

    public bool isPaused;
    float timeScaleOrig;


    void Awake()
    {
        instance = this;

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<playerController>();
        playerSpawnPosition = GameObject.Find("Player Spawn Position");

        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")  && currentMenu != playerDeadMenu && currentMenu != playerWinMenu)
        {
            isPaused = !isPaused;
            currentMenu = pauseMenu;
            currentMenu.SetActive(isPaused);

            if (isPaused)
                CursorLockPause();
            else
                CursorUnlockUnpause();
        }
    }

    public void CursorLockPause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(false);
        Time.timeScale = 0;
    }
    public void CursorUnlockUnpause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = timeScaleOrig;
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            reticle.SetActive(true);
        }
        currentMenu = null;
    }

    public void PlayerIsDead()
    {
        isPaused = true;
        playerDeadMenu.SetActive(true);
        currentMenu = playerDeadMenu;
        CursorLockPause();
    }

    public void enemyDecrement()
    {
        enemyCount--;
        remainingEnemiesLabel.text = enemyCount.ToString("F0");
        StartCoroutine(CheckEnemyCount());
    }

    public void enemyIncrement(int amount)
    {
        enemyCount += amount;
        remainingEnemiesLabel.text = enemyCount.ToString("F0");
    }

    IEnumerator CheckEnemyCount()
    {
        if (enemyCount <= 0)
        {
            yield return new WaitForSeconds(1);
            currentMenu = playerWinMenu;
            isPaused = true;
            currentMenu.SetActive(true);
            CursorLockPause();
        }
    }


}
