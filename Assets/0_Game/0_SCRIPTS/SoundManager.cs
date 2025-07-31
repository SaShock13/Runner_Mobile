using System;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    gameMusic,
    menuMusic,
    wrong,
    hit,
    jump,
    bonus,
    coin,
    diamond,
    speedBoost,
    invincibility,
    x2,
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
    [SerializeField] private AudioClip _wrongSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip[] _obstacleColliosionSounds;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _slideSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _bonusCollectSound;
    [SerializeField] private AudioClip _coinCollectSound;
    [SerializeField] private AudioClip _diamondCollectSound;
    [SerializeField] private AudioClip _speedBoostCollectSound;
    [SerializeField] private AudioClip _invincibilityCollectSound;
    [SerializeField] private AudioClip _x2CollectSound;
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
        _soundLibrary[Sounds.coin] = _coinCollectSound;
        _soundLibrary[Sounds.diamond] = _diamondCollectSound;
        _soundLibrary[Sounds.speedBoost] = _speedBoostCollectSound;
        _soundLibrary[Sounds.invincibility] = _invincibilityCollectSound;
        _soundLibrary[Sounds.x2] = _x2CollectSound;
        _soundLibrary[Sounds.wrong] = _wrongSound;
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
            //_sfxSource.clip = clip;
            _sfxSource.PlayOneShot(clip);
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

    internal void PlayObstacleCollision()
    {
        if (_obstacleColliosionSounds.Length == 0) return;
        _sfxSource.PlayOneShot(_obstacleColliosionSounds[UnityEngine.Random.Range(0, _obstacleColliosionSounds.Length)]);
    }
}
