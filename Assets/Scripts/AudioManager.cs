using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }
    
    // Singleton pattern
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Effects")]
    public SoundEffect throwSound;
    public SoundEffect bounceSound;
    public SoundEffect targetHitSound;
    public SoundEffect gameStartSound;
    public SoundEffect gameOverSound;
    
    [Header("Background Music")]
    public SoundEffect backgroundMusic;
    
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
        
        // Setup audio sources for each sound effect
        SetupAudioSource(ref throwSound);
        SetupAudioSource(ref bounceSound);
        SetupAudioSource(ref targetHitSound);
        SetupAudioSource(ref gameStartSound);
        SetupAudioSource(ref gameOverSound);
        SetupAudioSource(ref backgroundMusic);
        
        // Start background music
        if (backgroundMusic.clip != null)
        {
            PlayBackgroundMusic();
        }
    }
    
    private void SetupAudioSource(ref SoundEffect sound)
    {
        if (sound.clip == null)
            return;
            
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;
    }
    
    public void PlayThrowSound()
    {
        PlaySound(throwSound);
    }
    
    public void PlayBounceSound()
    {
        PlaySound(bounceSound);
    }
    
    public void PlayTargetHitSound()
    {
        PlaySound(targetHitSound);
    }
    
    public void PlayGameStartSound()
    {
        PlaySound(gameStartSound);
    }
    
    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic.source != null && !backgroundMusic.source.isPlaying)
        {
            backgroundMusic.source.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (backgroundMusic.source != null)
        {
            backgroundMusic.source.Stop();
        }
    }
    
    private void PlaySound(SoundEffect sound)
    {
        if (sound.source != null && sound.clip != null)
        {
            // Randomize pitch slightly for variety
            sound.source.pitch = sound.pitch * Random.Range(0.9f, 1.1f);
            sound.source.Play();
        }
    }
}
