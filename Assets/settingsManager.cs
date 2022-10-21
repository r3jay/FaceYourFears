using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class settingsManager : MonoBehaviour
{
    //ui navigation
    public GameObject optionsFirstButton;
    public GameObject optionsClosedButton;
    public GameObject creditsFirstButton;
    public GameObject creditsClosedButton;
    public GameObject credits2FirstButton;
    public GameObject credits2ClosedButton;

    public void showOptions()
    {

        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void closeOptions()
    {
        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void showCredits()
    {

        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(creditsFirstButton);
    }

    public void closeCredits()
    {
        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(creditsClosedButton);
    }

    public void showCredits2()
    {

        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(credits2FirstButton);
    }

    public void closeCredits2()
    {
        //setup navigation for ui
        //wipe first
        EventSystem.current.SetSelectedGameObject(null);
        //then set
        EventSystem.current.SetSelectedGameObject(credits2ClosedButton);
    }
}
