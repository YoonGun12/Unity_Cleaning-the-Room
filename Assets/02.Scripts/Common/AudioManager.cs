using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")] 
    public AudioClip bgmClip;
    public float bgmVolume;
    private AudioSource _bgmPlayer;

    [Header("SFX")] 
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    private AudioSource[] _sfxPlayers;
    private int _channelIndex;

    private void Awake()
    {
        instance = this;
        Init();
    }

    public enum Sfx {Jump, Magnet, SizeDown, SizeUp, Speedup, Timeup}
    
    void Init()
    {
        //배경음 플레이어 초기화
        var bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = bgmVolume;
        _bgmPlayer.clip = bgmClip;

        //효과음 플레이어 초기화
        var sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        _sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < _sfxPlayers.Length; i++)
        {
            _sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            _sfxPlayers[i].playOnAwake = false;
            _sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            _bgmPlayer.Play();
        }
        else
        {
            _bgmPlayer.Stop();
        }
    }
    
    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < _sfxPlayers.Length; i++)
        {
            var loopIndex = (_channelIndex + i) % _sfxPlayers.Length;
            if (_sfxPlayers[loopIndex].isPlaying)
                continue;

            _channelIndex = loopIndex;
            _sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            _sfxPlayers[loopIndex].Play();
            break;
        }
        
    }
}
