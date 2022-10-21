using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class resolutionSettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;

    List<string> resolution = new List<string>() {"1920 x 1080" ,"1024 x 768", "1152 x 864", "1280 x 720", "1280 x 768", "1280 x 800", "1280 x 960", "1280 x 1024", "1360 x 768", "1366 x 768",
     "1440 x 900", "1600 x 900", "1600 x 1024", "1600 x 1200", "1680 x 1050", "1920 x 1200", "1920 x 1440", "2048 x 1536", "2560 x 1440", "2560 x 1600", "3840 x 2160"};

    public const string WIDTH_KEY = "ResolutionWidth";
    public const string HEIGHT_KEY = "ResolutionHeight";

    int width;
    int height;

    void Awake()
    {
        populateResolutionDropdownList();
    }

    void Start()
    {
        width = PlayerPrefs.GetInt(WIDTH_KEY);
        height = PlayerPrefs.GetInt(HEIGHT_KEY);
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt(WIDTH_KEY, width);
        PlayerPrefs.SetInt(HEIGHT_KEY, height);
    }

    void populateResolutionDropdownList()
    {
        resolutionDropdown.AddOptions(resolution);
    }

    public void resolutionDropdownIndexChanged(int index)
    {
        if (resolutionDropdown.value == 0)
        {
            width = 1920;
            height = 1080;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 1)
        {
            Debug.Log("Dropdown index: " + index);
            width = 1024;
            height = 768;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 2)
        {
            width = 1152;
            height = 864;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 3)
        {
            width = 1280;
            height = 720;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 4)
        {
            width = 1280;
            height = 768;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }
        if (resolutionDropdown.value == 5)
        {
            width = 1280;
            height = 800;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 6)
        {
            width = 1280;
            height = 960;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 7)
        {
            width = 1280;
            height = 1024;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 8)
        {
            width = 1360;
            height = 768;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 9)
        {
            width = 1366;
            height = 768;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 10)
        {
            width = 1440;
            height = 900;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 11)
        {
            width = 1600;
            height = 900;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 12)
        {
            width = 1600;
            height = 1024;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 13)
        {
            width = 1600;
            height = 1200;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 14)
        {
            width = 1680;
            height = 1050;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 15)
        {
            width = 1920;
            height = 1200;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 16)
        {
            width = 1920;
            height = 1440;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 17)
        {
            width = 2048;
            height = 1536;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 18)
        {
            width = 2560;
            height = 1440;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 19)
        {
            width = 2560;
            height = 1600;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        if (resolutionDropdown.value == 20)
        {
            width = 3840;
            height = 2160;
            Screen.SetResolution(width, height, true);
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }
    }
}
