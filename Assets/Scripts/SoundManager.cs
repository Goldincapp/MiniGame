using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonComponent<SoundManager>
{
    [SerializeField] private List<AudioSource> _asteroidDestroySounds = new List<AudioSource>();
    [SerializeField] private List<AudioSource> _laserSounds = new List<AudioSource>();
    [SerializeField] private AudioSource _shipDestroySound;
    [SerializeField] private AudioSource _engineSound;
    [SerializeField] private AudioSource _friendDestroySound;
    [SerializeField] private AudioSource _enemyDestroySound;
    [SerializeField] private AudioSource _menuButtonSound;
    [Header("Music")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private float _menuMusicVolume;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private float _gameMusicVolume;

    public void PlayAsteroidDestroySound() => _asteroidDestroySounds[Random.Range(0, _asteroidDestroySounds.Count)].Play();

    public void PlayLaserSound() => _laserSounds[Random.Range(0, _laserSounds.Count)].Play();

    public void PlayShipDestroySound() => _shipDestroySound.Play();

    public void PlayEngineSound() => _engineSound.Play();

    public void StopEngineSound() => _engineSound.Stop();

    public void PlayFriendDestroySound() => _friendDestroySound.Play();

    public void PlayEnemyDestroySound() => _enemyDestroySound.Play();

    public void PlayMenuButtonSound() => _menuButtonSound.Play();

    public void PlayMenuMusic()
    {
        _musicSource.clip = _menuMusic;
        _musicSource.volume = _menuMusicVolume;
        _musicSource.Play();
    }

    public void PlayGameMusic()
    {
        _musicSource.clip = _gameMusic;
        _musicSource.volume = _gameMusicVolume;
        _musicSource.Play();
    }
}
