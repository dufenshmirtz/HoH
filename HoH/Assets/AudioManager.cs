using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource SFX;

    public List<AudioClip> playlist;
    public List<AudioClip> menuPlaylist;

    //clips
    public AudioClip death;
    public AudioClip heavyattack;
    public AudioClip lightattack;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip backround;
    public AudioClip swoosh;
    public AudioClip heavyswoosh;
    public AudioClip run;
    public AudioClip dash;
    public AudioClip dashHit;
    public AudioClip counterScream;
    public AudioClip counterClong;

    //volumes
    public float deathVolume = 1.0f;
    public float heavyAttackVolume = 1.0f;
    public float lightAttackVolume = 1.0f;
    public float jumpVolume = 1.0f;
    public float landVolume = 1.0f;
    public float swooshVolume = 1.0f;
    public float heavySwooshVolume = 1.0f;
    public float runVolume = 1.0f;
    public float counterVol = 1.0f;
    public float counterClongVol = 20.0f;


    //settings
    private static string musicPref = "MusicPref";
    private static string sfxPref = "SfxPref";
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;

    private void Start()
    {
        if (playlist.Count > 0)
        {
            // Select a random index from the playlist
            int randomIndex = Random.Range(0, playlist.Count);

            // Get the randomly selected AudioClip
            AudioClip selectedSong = playlist[randomIndex];

            // Assign the selected song to the music AudioSource
            music.clip = selectedSong;

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


    public void PlaySFX(AudioClip sfx,float volume)
    {
        SFX.PlayOneShot(sfx,volume);
    }

    private void Awake()
    {
        ContinueSettings();
    }

    private void ContinueSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(musicPref);
        sfxVolume = PlayerPrefs.GetFloat(sfxPref);

        music.volume = musicVolume;
        SFX.volume = sfxVolume;
    }

}
