using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*This file creates the functions for all the buttons used within
the menus. Some of the functions have been commented out to preserve
the functionality of the code while other pieces are being uploaded:
-line 24
-line 30
-line 36
-line 47
-line 52
-line 57
*/
public class buttonFunctions : MonoBehaviour
{
    //for button that resumes the game
    public void resume()
    {
        if (gameManager.instance.isPaused)
        {
            gameManager.instance.isPaused = !gameManager.instance.isPaused;
            gameManager.instance.cursorUnlockUnpause();
        }
    }

    public void restart()
    {
		gameManager.instance.isPaused = false;
        gameManager.instance.cursorUnlockUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void playerRespawn()
    {
        gameManager.instance.playerScript.respawn();
		gameManager.instance.isPaused = false;
    }

    public void quit()
    {
        Application.Quit();
    }

    //button to give player HP
    public void giveHP(int amount)
    {
       gameManager.instance.playerScript.giveHP(amount);
    }

    public void giveJump(int amount)
    {
       gameManager.instance.playerScript.giveJump(amount);
    }

    public void giveSpeed(int amount)
    {
       gameManager.instance.playerScript.giveSpeed(amount);
    }
}
