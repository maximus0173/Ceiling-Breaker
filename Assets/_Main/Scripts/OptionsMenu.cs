using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{

    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public AudioMixer audioMixer;
    public UnityEvent backEvent;

    private void Start()
    {
        this.musicVolumeSlider.onValueChanged.AddListener(MusicVolumeChanged);
        this.soundVolumeSlider.onValueChanged.AddListener(SoundVolumeChanged);
    }

    public void OnEnable()
    {
        this.audioMixer.GetFloat("MainMusicVolume", out float musicVolumeRaw);
        float musicVolume = Mathf.Pow(10.0f, musicVolumeRaw / 20.0f);
        this.musicVolumeSlider.value = musicVolume;

        this.audioMixer.GetFloat("MainSFXVolume", out float soundVolumeRaw);
        float soundVolume = Mathf.Pow(10.0f, soundVolumeRaw / 20.0f);
        this.soundVolumeSlider.value = soundVolume;
    }

    public void BackClicked()
    {
        this.backEvent.Invoke();
    }

    public void MusicVolumeChanged(float newVolume)
    {
        float newMusicVolume = Mathf.Clamp(Mathf.Log10(this.musicVolumeSlider.value) * 20f, -80f, 0f);
        this.audioMixer.SetFloat("MainMusicVolume", newMusicVolume);
    }

    public void SoundVolumeChanged(float newVolume)
    {
        float newSoundVolume = Mathf.Clamp(Mathf.Log10(this.soundVolumeSlider.value) * 20f, -80f, 0f);
        this.audioMixer.SetFloat("MainSFXVolume", newSoundVolume);
    }

}
