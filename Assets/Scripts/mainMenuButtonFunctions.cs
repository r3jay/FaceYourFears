using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class mainMenuButtonFunctions : MonoBehaviour
{

    [SerializeField] GameObject buttonSound;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject credits2;
    [SerializeField] GameObject options;
    public void newGame()
    {
        SceneManager.LoadScene(1);
    }
    public void showOptions()
    {
        mainMenu.SetActive(false);
        Instantiate(buttonSound);
        options.SetActive(true);
    }

    public void closeOptions()
    {
        mainMenu.SetActive(true);
        Instantiate(buttonSound);
        options.SetActive(false);

    }

    public void showCredits()
    {
        mainMenu.SetActive(false);
        Instantiate(buttonSound);
        credits.SetActive(true);
    }

    public void closeCredits()
    {
        mainMenu.SetActive(true);
        Instantiate(buttonSound);
        credits.SetActive(false);
    }

    public void showCredits2()
    {
        credits.SetActive(false);
        Instantiate(buttonSound);
        credits2.SetActive(true);
    }

    public void closeCredits2()
    {
        credits.SetActive(true);
        Instantiate(buttonSound);
        credits2.SetActive(false);
    }

    public void quit()
    {
        Instantiate(buttonSound);
        Application.Quit();
    }
}
