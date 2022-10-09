using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    [SerializeField] AudioMixer mixer;
    public const string MENU_MUSIC_KEY = "menuMusicVolume";
    public const string MENU_SFX_KEY = "menuSFXVolume";
    public const string GAME_MUSIC_KEY = "gameMusicVolume";
    public const string GAME_SFX_KEY = "gameSFXVolume";

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        LoadVol();
    }


    void LoadVol()
    {
        float menuMusicVol = PlayerPrefs.GetFloat(MENU_MUSIC_KEY, 1f);
        float menuSFXVol = PlayerPrefs.GetFloat(MENU_SFX_KEY, 1f);
        float gameMusicVol = PlayerPrefs.GetFloat(GAME_MUSIC_KEY, 1f);
        float gameSFXVol = PlayerPrefs.GetFloat(GAME_SFX_KEY, 1f);

        mixer.SetFloat(volumeSettings.MIXER_MENUMUSIC, Mathf.Log10(menuMusicVol) * 20);
        mixer.SetFloat(volumeSettings.MIXER_MENUSFX, Mathf.Log10(menuSFXVol) * 20);
        mixer.SetFloat(volumeSettings.MIXER_GAME_MUSIC, Mathf.Log10(gameMusicVol) * 20);
        mixer.SetFloat(volumeSettings.MIXER_GAME_SFX, Mathf.Log10(gameSFXVol) * 20);
    }
}
