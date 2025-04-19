using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Audio : MonoBehaviour
{
    public static Audio instance;

    public AudioSource pickUp, button, playerBounce, wings, UnlockBird,
        jetPack, mallet, choir, slowTime, pinks, oranges, goggles, 
        sticky, walls, stickyPickUp, addLives, fail, switchBird, newHighScore, addSkipAdTokens, ProgressBar;

    public AudioMixer audioMixer;

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    public AudioClip Music1, Music2;

    //sdd

    private void Start()
    {
        instance = this;

        // use saved volumed if settings if available
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            AdjustMusicVolume();

            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            AdjustSFXvolume();
        }
    }

    public void AdjustMusicVolume() 
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolumeSlider.value) * 20); // Convert linear to dB
    }

    public void AdjustSFXvolume()
    {
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(sfxVolumeSlider.value) * 20); // Convert linear to dB
    }

    public void SaveVolumeLevels()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
  
}
