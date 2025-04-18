using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Sound Effects")]
    public AudioClip throwSound;
    public AudioClip bounceSound;
    public AudioClip targetHitSound;
    public AudioClip scoreSound;
    public AudioClip gameOverSound;
    public AudioClip buttonClickSound;
    public AudioClip targetPlacedSound;
    
    [Header("Background Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    public bool enableMusic = true;
    public bool enableSFX = true;
    
    // Singleton pattern
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Initialize audio sources
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        // Set initial volumes
        UpdateVolumes();
        
        // Start playing menu music
        PlayMenuMusic();
    }
    
    // Volume control
    public void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = enableMusic ? musicVolume : 0f;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = enableSFX ? sfxVolume : 0f;
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        
        // Save volume setting
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        
        // Save volume setting
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    
    public void ToggleMusic(bool enabled)
    {
        enableMusic = enabled;
        UpdateVolumes();
        
        // Save setting
        PlayerPrefs.SetInt("MusicEnabled", enableMusic ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void ToggleSFX(bool enabled)
    {
        enableSFX = enabled;
        UpdateVolumes();
        
        // Save setting
        PlayerPrefs.SetInt("SFXEnabled", enableSFX ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // Music playback
    public void PlayMenuMusic()
    {
        if (menuMusic != null && musicSource != null)
        {
            if (musicSource.clip != menuMusic)
            {
                musicSource.clip = menuMusic;
                
                if (enableMusic)
                {
                    musicSource.Play();
                }
            }
        }
    }
    
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null && musicSource != null)
        {
            if (musicSource.clip != gameplayMusic)
            {
                musicSource.clip = gameplayMusic;
                
                if (enableMusic)
                {
                    musicSource.Play();
                }
            }
        }
    }
    
    // Sound effect methods
    public void PlayThrowSound()
    {
        PlaySFX(throwSound);
    }
    
    public void PlayBounceSound()
    {
        PlaySFX(bounceSound);
    }
    
    public void PlayTargetHitSound()
    {
        PlaySFX(targetHitSound);
    }
    
    public void PlayScoreSound()
    {
        PlaySFX(scoreSound);
    }
    
    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }
    
    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlayTargetPlacedSound()
    {
        PlaySFX(targetPlacedSound);
    }
    
    // Generic method to play a sound effect
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null && enableSFX)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }
    
    // Load saved audio settings
    private void LoadAudioSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        enableMusic = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        enableSFX = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        
        // Apply loaded settings
        UpdateVolumes();
    }
}