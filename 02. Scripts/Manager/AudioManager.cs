using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public enum BGM
{
    StartSceneMusic
}

public enum SFX
{
    FearGhost,
    FearBoom,
    UIHandle,
    Rewind,
    ChurchBell
}

public enum HEART
{
    Heartbeat,
}

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] List<AudioClip> bgmList = new List<AudioClip>();

    [Header("Effect Sound")]
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] List<AudioClip> sfxList = new List<AudioClip>();
    
    [SerializeField] AudioSource heartAudioSource;
    [SerializeField] List<AudioClip> heartList = new List<AudioClip>();
    
    [SerializeField] private float masterVol = 1;
    [SerializeField] private float bgmVol = 1;
    [SerializeField] private float sfxVol = 1;
    [SerializeField] private float heartSFXVol = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlayBGM(BGM _bgm)
    {
        if (bgmAudioSource.clip == bgmList[(int)_bgm])
            return;
        bgmAudioSource.loop = true;
        bgmAudioSource.clip = bgmList[(int)_bgm];
        bgmAudioSource.Play();
    }

    public void PlaySFX(SFX sfx)
    {
        sfxAudioSource.PlayOneShot(sfxList[(int)sfx]);
    }

    // 레코더 재생
    public void PlayRecorder()
    {
        sfxAudioSource.clip = sfxList[(int)SFX.Rewind];
        sfxAudioSource.pitch = 1f;
        sfxAudioSource.loop = true;
        sfxAudioSource.Play();
    }

    // 레코더 중단
    public void StopRecorder()
    {
        sfxAudioSource.Stop();
    }
    
    public void Playheart(HEART heart)
    {
        heartAudioSource.PlayOneShot(heartList[(int)heart]);
    }

    public void PlaySFXDirect(AudioClip clip)
    {
        sfxAudioSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(vol) * 20f);
    }

    public void SetBGMVolume(float vol)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(vol) * 20f);
    }

    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(vol) * 20f);
    }
    
    public void SetHeartVolume(float vol)
    {
        audioMixer.SetFloat("HEART", Mathf.Log10(vol) * 20f);
    }

    public BGM GetCurrentPlayingBGMClip()
    {
        return (BGM)bgmList.FindIndex(clip => clip == bgmAudioSource.clip);
    }
}