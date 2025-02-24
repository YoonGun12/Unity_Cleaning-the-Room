using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")] 
    public AudioClip[] bgmClips;
    public float bgmVolume;
    private AudioSource _bgmPlayer;
    private Bgm currentBgm = Bgm.Title;

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

    public enum Sfx
    {
        Jump,
        Magnet,
        SizeDown,
        SizeUp,
        Speedup,
        Timeup,
        TimeOut,
        ButtonClick,
        FootStep,
        Landing,
        AddTime,
        PowerUp,
        HardObject,
        Kick
    }

    public enum Bgm
    {
        Title,
        InGame1,
        InGame2,
        InGame3,
        Result
    }

    void Init()
    {
        //배경음 플레이어 초기화
        var bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = bgmVolume;

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

    public void PlayBgm(Bgm bgmType ,bool isPlay)
    {
        if (isPlay)
        {
            if (currentBgm == bgmType && _bgmPlayer.isPlaying)
                return;
            _bgmPlayer.clip = bgmClips[(int)bgmType];
            _bgmPlayer.Play();
            currentBgm = bgmType;
        }
        else
        {
            if (currentBgm == bgmType && _bgmPlayer.isPlaying)
            {
                _bgmPlayer.Stop();
            }
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
