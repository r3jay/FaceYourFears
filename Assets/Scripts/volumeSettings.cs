using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



public class volumeSettings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider masterSlider;

    public const string MIXER_SFX = "SfxVolume";
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_MASTER = "MasterVolume";

    public const string SFX_KEY = "SFXVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string MASTER_KEY = "MasterVolume";

    void Awake()
    {
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
        musicSlider.onValueChanged.AddListener(setMusicVolume);
        masterSlider.onValueChanged.AddListener(setMasterVolume);
    }
    void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);
        PlayerPrefs.SetFloat(MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(MASTER_KEY, masterSlider.value);
    }

    public void setSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);

    }
    public void setMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, musicSlider.value);

    }

    public void setMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MASTER_KEY, masterSlider.value);
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, musicSlider.value);

    }
}
