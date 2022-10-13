using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class resolutionSettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;

    List<string> resolution = new List<string>() {"1024 x 768", "1152 x 864", "1280 x 720", "1280 x 768", "1280 x 800", "1280 x 960", "1280 x 1024", "1360 x 768", "1366 x 768",
     "1440 x 900", "1600 x 900", "1600 x 1024", "1600 x 1200", "1680 x 1050", "1920 x 1080", "1920 x 1200", "1920 x 1440", "2048 x 1536", "2560 x 1440", "2560 x 1600", "3840 x 2160"};

    void Awake()
    {
        populateResolutionDropdownList();
        resolutionDropdown.value = 14;
    }

    void populateResolutionDropdownList()
    {
        resolutionDropdown.AddOptions(resolution);
    }

    public void resolutionDropdownIndexChanged(int index)
    {
        if (resolutionDropdown.value == 0)
        {
            Screen.SetResolution(1024, 768, true);
        }

        if (resolutionDropdown.value == 1)
        {
            Screen.SetResolution(1152, 864, true);
        }

        if (resolutionDropdown.value == 2)
        {
            Screen.SetResolution(1280, 720, true);
        }

        if (resolutionDropdown.value == 3)
        {
            Screen.SetResolution(1280, 768, true);
        }
        if (resolutionDropdown.value == 4)
        {
            Screen.SetResolution(1280, 800, true);
        }

        if (resolutionDropdown.value == 5)
        {
            Screen.SetResolution(1280, 960, true);
        }

        if (resolutionDropdown.value == 6)
        {
            Screen.SetResolution(1280, 1024, true);
        }

        if (resolutionDropdown.value == 7)
        {
            Screen.SetResolution(1360, 768, true);
        }

        if (resolutionDropdown.value == 8)
        {
            Screen.SetResolution(1366, 768, true);
        }

        if (resolutionDropdown.value == 9)
        {
            Screen.SetResolution(1440, 900, true);
        }

        if (resolutionDropdown.value == 10)
        {
            Screen.SetResolution(1600, 900, true);
        }

        if (resolutionDropdown.value == 11)
        {
            Screen.SetResolution(1600, 1024, true);
        }

        if (resolutionDropdown.value == 12)
        {
            Screen.SetResolution(1600, 1200, true);
        }

        if (resolutionDropdown.value == 13)
        {
            Screen.SetResolution(1680, 1050, true);
        }

        if (resolutionDropdown.value == 14)
        {
            Screen.SetResolution(1920, 1080, true);
        }

        if (resolutionDropdown.value == 15)
        {
            Screen.SetResolution(1920, 1200, true);
        }

        if (resolutionDropdown.value == 16)
        {
            Screen.SetResolution(1920, 1440, true);
        }

        if (resolutionDropdown.value == 17)
        {
            Screen.SetResolution(2048, 1536, true);
        }

        if (resolutionDropdown.value == 18)
        {
            Screen.SetResolution(2560, 1440, true);
        }

        if (resolutionDropdown.value == 19)
        {
            Screen.SetResolution(2560, 1600, true);
        }

        if (resolutionDropdown.value == 20)
        {
            Screen.SetResolution(3840, 2160, true);
        }
    }
}
