
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class volumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider menuMusicSlider;
    [SerializeField] Slider menuSfxSlider;
    [SerializeField] Slider gameMusicSlider;
    [SerializeField] Slider gameSfxSlider;

    public const string MIXER_MENUMUSIC = "MenuMusicVolume";
    public const string MIXER_MENUSFX = "MenuSFXVolume";
    public const string MIXER_GAME_MUSIC = "GameMusicVolume";
    public const string MIXER_GAME_SFX = "GameSFXVolume";

    void Awake()
    {
        menuMusicSlider.onValueChanged.AddListener(setMenuMusicVolume);
        menuSfxSlider.onValueChanged.AddListener(setMenuSFXVolume);
        gameMusicSlider.onValueChanged.AddListener(setGameMusicVolume);
        gameSfxSlider.onValueChanged.AddListener(setGameMusicVolume);
    }
    void Start()
    {
        menuMusicSlider.value = PlayerPrefs.GetFloat(audioManager.MENU_MUSIC_KEY, 1f);
        menuSfxSlider.value = PlayerPrefs.GetFloat(audioManager.MENU_SFX_KEY, 1f); 
        gameMusicSlider.value = PlayerPrefs.GetFloat(audioManager.GAME_MUSIC_KEY, 1f);
        gameSfxSlider.value = PlayerPrefs.GetFloat(audioManager.GAME_SFX_KEY, 1f);

    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(audioManager.MENU_MUSIC_KEY, menuMusicSlider.value);
        PlayerPrefs.SetFloat(audioManager.MENU_SFX_KEY, menuSfxSlider.value);
        PlayerPrefs.SetFloat(audioManager.GAME_MUSIC_KEY, gameMusicSlider.value);
        PlayerPrefs.SetFloat(audioManager.GAME_SFX_KEY, gameSfxSlider.value);
    }

    void setMenuMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MENUMUSIC, Mathf.Log10(value) * 20);
    }

    void setMenuSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_MENUSFX, Mathf.Log10(value) * 20);
    }
    void setGameMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_GAME_MUSIC, Mathf.Log10(value) * 20);
    }

    void setGameSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_GAME_SFX, Mathf.Log10(value) * 20);
    }
}
