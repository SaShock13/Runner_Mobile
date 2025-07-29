using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    gameMusic,
    menuMusic,
    hit,
    jump,
    bonus,
    death,
    slide
}

// todo сделать пул сорсов и вообще доработать 
public class SoundManager : MonoBehaviour
{
    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    [SerializeField] private AudioClip gamedMusic;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _slideSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _bonusCollectSound;
    [SerializeField, Range (0,1)] private float sfxVolume;
    [SerializeField, Range(0, 1)] private float musicVolume;

    private Dictionary<Sounds,AudioClip> _soundLibrary = new Dictionary<Sounds,AudioClip>();

    public void Start()
    {
        _soundLibrary[Sounds.gameMusic] = gamedMusic;
        _soundLibrary[Sounds.menuMusic] = _menuMusic;
        _soundLibrary[Sounds.hit] = _hitSound;
        _soundLibrary[Sounds.jump] = _jumpSound;
        _soundLibrary[Sounds.slide] = _slideSound;
        _soundLibrary[Sounds.death] = _deathSound;
        _soundLibrary[Sounds.bonus] = _bonusCollectSound;
        _musicSource.loop = true;
        _musicSource.volume = 1;
        _musicSource. volume = musicVolume;
        _sfxSource.volume = sfxVolume;
        PlayMusic(Sounds.menuMusic);
    }

    private void Awake()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySFX(Sounds sound)
    {
        if (_soundLibrary.TryGetValue(sound, out AudioClip clip))
        {
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }
    }

    public void PlayMusic(Sounds sound)
    {
        if (_soundLibrary.TryGetValue(sound, out AudioClip clip))
        {
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void SetSFXVolume(float vol)
    {
        _sfxSource.volume = vol;
    } 
    
    public void SetMusicVolume(float vol)
    {
        _musicSource.volume = vol;
    }
}
