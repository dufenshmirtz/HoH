using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMusic : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sfx;
    public List<AudioClip> menuplaylist;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public Slider musicSlider, sfxSlider;
    private static string firstPlay = "FirstPlay";
    private static string musicPref = "MusicPref";
    private static string sfxPref = "SfxPref";
    int firstPlayInt;
    

    private void Start()
    {

        firstPlayInt=PlayerPrefs.GetInt(firstPlay);

        if(firstPlayInt==0)
        {
            musicVolume = 1.0f;
            sfxVolume = 1.0f;
            musicSlider.value = 1.0f;
            sfxSlider.value = 1.0f;
            PlayerPrefs.SetFloat(musicPref, musicVolume);
            PlayerPrefs.SetFloat(sfxPref, sfxVolume);
            PlayerPrefs.SetInt(firstPlay, -1);
        }
        else
        {
            musicVolume=PlayerPrefs.GetFloat(musicPref);
            musicSlider.value=musicVolume;

            

            sfxVolume=PlayerPrefs.GetFloat(sfxPref);
            sfxSlider.value = sfxVolume;
        }

        if (menuplaylist.Count > 0)
        {
            // Select a random index from the playlist
            int randomIndex = Random.Range(0, menuplaylist.Count);

            // Get the randomly selected AudioClip
            AudioClip selectedSong = menuplaylist[randomIndex];

            // Assign the selected song to the music AudioSource
           music.clip = selectedSong;

            // Set the music volume
            music.volume = musicVolume;

            // Play the selected song
            music.Play();
        }
        else
        {
            Debug.LogError("Playlist is empty. Add AudioClips to the playlist in the Inspector.");
        }
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(musicPref,musicSlider.value);
        PlayerPrefs.SetFloat(sfxPref, sfxSlider.value);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveSoundSettings();
        }
    }

    public void UpdateSound()
    {
        music.volume=musicSlider.value;
        sfx.volume =sfxSlider.value;
        SaveSoundSettings();
    }


}
